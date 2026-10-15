using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerritoryView : MonoBehaviour
{
    private const int AreaSortingOrder = -10;
    private const float MinAreaTint = 0.2f;
    private int lastShownCount = -1;

    private Material areaMaterial;
    private Color areaFrom;
    private Color areaTo;

    [SerializeField] private SpriteRenderer body;
    [SerializeField] private TMP_Text countLabel;
    [SerializeField] private MeshFilter areaFilter;
    [SerializeField] private MeshRenderer areaRenderer;
    [SerializeField] private float radius = 0.5f;

    private Territory territory;

    public void Bind(Territory territory, FactionPalette palette)
    {
        this.territory = territory;
        this.territory.CountChanged += OnCountChanged;

        body.color = palette.GetDotColor(territory.OwnerId);
        body.transform.localScale = Vector3.one * (radius * 2f);
        BuildArea(palette);
        Refresh();
    }

    private void OnDestroy()
    {
        if (territory != null)
            territory.CountChanged -= OnCountChanged;

        if (areaFilter != null && areaFilter.sharedMesh != null)
            Destroy(areaFilter.sharedMesh);

        if (areaMaterial != null)
            Destroy(areaMaterial);
    }

    private void OnCountChanged(Territory changed)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (lastShownCount != territory.Count)
        {
            lastShownCount = territory.Count;
            countLabel.text = territory.Count.ToString();
        }

        areaMaterial.color = Color.Lerp(areaFrom, areaTo, territory.FillRatio);
    }

    private void BuildArea(FactionPalette palette)
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

        areaMaterial = areaRenderer.material;

        areaTo = palette.GetAreaColor(territory.OwnerId);
        areaFrom = Color.Lerp(Color.white, areaTo, MinAreaTint);

        areaRenderer.sortingOrder = AreaSortingOrder;
    }
}