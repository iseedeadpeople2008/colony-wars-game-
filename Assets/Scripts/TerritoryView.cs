using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerritoryView : MonoBehaviour
{
    private const int AreaSortingOrder = -10;
    private const float MinAreaTint = 0.4f;

    private Territory territory;
    private int lastShownCount = -1;

    private static readonly int ColorProp = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock propBlock;
    private Color areaFrom;
    private Color areaTo;
    private FactionPalette palette;

    [SerializeField] private SpriteRenderer body;
    [SerializeField] private TMP_Text countLabel;
    [SerializeField] private MeshFilter areaFilter;
    [SerializeField] private MeshRenderer areaRenderer;
    [SerializeField] private float radius = 0.5f;

    public void Bind(Territory territory, FactionPalette palette)
    {
        this.territory = territory;
        this.territory.CountChanged += OnCountChanged;
        this.territory.OwnerChanged += OnOwnerChanged;

        this.palette = palette;

        propBlock = new MaterialPropertyBlock();

        body.transform.localScale = Vector3.one * (radius * 2f);
        BuildArea();
        ApplyColors();
        Refresh();
    }

    private void OnDestroy()
    {
        if (territory != null)
        {
            territory.CountChanged -= OnCountChanged;
            territory.OwnerChanged -= OnOwnerChanged;
        }

        if (areaFilter != null && areaFilter.sharedMesh != null)
            Destroy(areaFilter.sharedMesh);
    }

    private void OnCountChanged(Territory changed)
    {
        Refresh();
    }

    private void ApplyColors()
    {
        body.color = palette.GetDotColor(territory.OwnerId);

        areaTo = palette.GetAreaColor(territory.OwnerId);
        areaFrom = Color.Lerp(Color.white, areaTo, MinAreaTint);
    }

    private void Refresh()
    {
        if (lastShownCount != territory.Count)
        {
            lastShownCount = territory.Count;
            countLabel.SetText("{0}", territory.Count);
        }

        areaRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(ColorProp, Color.Lerp(areaFrom, areaTo, territory.FillRatio));
        areaRenderer.SetPropertyBlock(propBlock);
    }

    private void OnOwnerChanged(Territory changed)
    {
        ApplyColors();
        Refresh();
    }

    private void BuildArea()
    {
        IReadOnlyList<Vector2> points = territory.Shape.Points;

        var vertices = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
            vertices[i] = points[i] - territory.Position;

        var triangles = new int[(points.Count - 2) * 3];
        for (int i = 0; i < points.Count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        areaFilter.sharedMesh = mesh;

        areaRenderer.sortingOrder = AreaSortingOrder;
    }
}