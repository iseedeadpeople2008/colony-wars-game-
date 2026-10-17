using System.Collections.Generic;
using UnityEngine;
using System;

public class SoldierLayerView : MonoBehaviour
{
    [SerializeField] private Mesh soldierMesh;          
    [SerializeField] private Material soldierMaterial;  
    [SerializeField] private float soldierScale = 0.3f;

    private readonly Matrix4x4[] matrices = new Matrix4x4[1023];
    private readonly Vector4[] colors = new Vector4[1023];
    private MaterialPropertyBlock propBlock;
    private static readonly int ColorProp = Shader.PropertyToID("_Color");

    private GameWorld world;
    private FactionPalette palette;

    public void Bind(GameWorld world, FactionPalette palette)
    {
        this.world = world;
        this.palette = palette;
        
        propBlock = new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        if (world == null || soldierMesh == null || soldierMaterial == null)
            return;

        IReadOnlyList<Soldier> soldiers = world.Soldiers;
        int count = Math.Min(soldiers.Count, 1023);
        if (count == 0)
            return;

        Vector3 scale = new(soldierScale, soldierScale, 1f);

        for (int i = 0; i < count; i++)
        {
            matrices[i].SetTRS(soldiers[i].Position, Quaternion.identity, scale);
            colors[i] = palette.GetDotColor(soldiers[i].FactionId);
        }

        propBlock.SetVectorArray(ColorProp, colors);
        Graphics.DrawMeshInstanced(soldierMesh, 0, soldierMaterial, matrices, count, propBlock);
    }
}