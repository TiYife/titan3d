﻿using EngineNS.Graphics.Mesh;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace EngineNS.GamePlay.Scene
{
    [Bricks.CodeBuilder.ContextMenu("Sun", "Sun", UNode.EditorKeyword)]
    [UNode(NodeDataType = typeof(TtSunNode.TtSunNodeData), DefaultNamePrefix = "Sun")]
    public class TtSunNode : USceneActorNode
    {
        public class TtSunNodeData : UNodeData
        {
            public TtSunNodeData()
            {
                SunMaterialName = RName.GetRName("material/default_sun.uminst", RName.ERNameType.Engine);
            }
            public TtDirectionLight DirectionLight { get; set; } = new TtDirectionLight();
            [Rtti.Meta]
            [RName.PGRName(FilterExts = Graphics.Pipeline.Shader.UMaterialInstance.AssetExt)]
            public RName SunMaterialName { get; set; }
        }
        protected override void OnParentChanged(UNode prev, UNode cur)
        {
            if (cur == null)
            {
                prev.GetWorld().mSuns.Remove(this);
            }
            base.OnParentChanged(prev, cur);
        }
        [Category("Option")]
        [Rtti.Meta]
        public TtDirectionLight DirectionLight
        {
            get => GetNodeData<TtSunNodeData>().DirectionLight;
            set => GetNodeData<TtSunNodeData>().DirectionLight = value;
        }
        Graphics.Pipeline.Shader.UMaterialInstance SunMaterial;
        [Category("Option")]
        [Rtti.Meta]
        [RName.PGRName(FilterExts = Graphics.Pipeline.Shader.UMaterialInstance.AssetExt)]
        public RName SunMaterialName
        {
            get => GetNodeData<TtSunNodeData>().SunMaterialName;
            set
            {
                GetNodeData<TtSunNodeData>().SunMaterialName = value;
                var action = async () =>
                {
                    SunMaterial = await UEngine.Instance.GfxDevice.MaterialInstanceManager.GetMaterialInstance(value);
                };
                action();
            }
        }
        public override async Task<bool> InitializeNode(UWorld world, UNodeData data, EBoundVolumeType bvType, Type placementType)
        {
            await base.InitializeNode(world, data, bvType, placementType);

            world.mSuns.Add(this);
            return true;
        }
        public override void OnGatherVisibleMeshes(UWorld.UVisParameter rp)
        {
            base.OnGatherVisibleMeshes(rp);

            if (DebugMesh != null)
                rp.VisibleMeshes.Add(DebugMesh);
        }
        public override void GetHitProxyDrawMesh(List<UMesh> meshes)
        {
            base.GetHitProxyDrawMesh(meshes);
        }

        protected override void OnAbsTransformChanged()
        {
            base.OnAbsTransformChanged();
            DirectionLight.Direction = Quaternion.RotateVector3(in Placement.TransformRef.mQuat, in Vector3.UnitX);
        }

        Graphics.Mesh.UMesh mDebugMesh;
        public Graphics.Mesh.UMesh DebugMesh
        {
            get
            {
                if (mDebugMesh == null)
                {
                    var cookedMesh = UEngine.Instance.GfxDevice.MeshPrimitiveManager.FindMeshPrimitive(RName.GetRName("axis/movex.vms", RName.ERNameType.Engine));
                    if (cookedMesh == null)
                        return null;
                    var materials1 = new Graphics.Pipeline.Shader.UMaterialInstance[1];
                    materials1[0] = UEngine.Instance.GfxDevice.MaterialInstanceManager.FindMaterialInstance(RName.GetRName("axis/axis_x_d.uminst", RName.ERNameType.Engine));
                    var mesh2 = new Graphics.Mesh.UMesh();
                    var ok1 = mesh2.Initialize(cookedMesh, materials1,
                        Rtti.UTypeDescGetter<Graphics.Mesh.UMdfStaticMeshPermutation<Graphics.Pipeline.Shader.UMdf_NoShadow>>.TypeDesc);
                    if (ok1)
                    {
                        mDebugMesh = mesh2;

                        mDebugMesh.HostNode = this;

                        BoundVolume.LocalAABB = mDebugMesh.MaterialMesh.Mesh.mCoreObject.mAABB;

                        this.HitproxyType = Graphics.Pipeline.UHitProxy.EHitproxyType.Root;

                        UpdateAbsTransform();
                        UpdateAABB();
                        Parent?.UpdateAABB();
                    }
                }
                return mDebugMesh;
            }
        }
    }
}
