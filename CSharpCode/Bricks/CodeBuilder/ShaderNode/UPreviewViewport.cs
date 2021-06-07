﻿using System;
using System.Collections.Generic;
using SDL2;

namespace EngineNS.Bricks.CodeBuilder.ShaderNode
{
    public class UPreviewViewport : Graphics.Pipeline.UViewportSlate
    {
        public Editor.Controller.EditorCameraController CameraController = new Editor.Controller.EditorCameraController();
        public UPreviewViewport(bool regRoot)
            : base(regRoot)
        {

        }
        ~UPreviewViewport()
        {
            Cleanup();
        }
        public void Cleanup()
        {
            RenderPolicy?.Cleanup();
            RenderPolicy = null;
            if (SnapshotPtr != IntPtr.Zero)
            {
                var handle = System.Runtime.InteropServices.GCHandle.FromIntPtr(SnapshotPtr);
                handle.Free();
                SnapshotPtr = IntPtr.Zero;
            }
        }
        IntPtr SnapshotPtr;
        public virtual async System.Threading.Tasks.Task Initialize(Graphics.Pipeline.USlateApplication application, Graphics.Pipeline.IRenderPolicy policy, float zMin, float zMax)
        {
            RenderPolicy = policy;

            await RenderPolicy.Initialize(1, 1);

            CameraController.Camera = RenderPolicy.GBuffers.Camera;

            var materials = new Graphics.Pipeline.Shader.UMaterial[1];
            materials[0] = await UEngine.Instance.GfxDevice.MaterialManager.GetMaterial(RName.GetRName("utest/ttt.material"));
            if (materials[0] == null)
                return;
            var mesh = new Graphics.Mesh.UMesh();
            var rect = Graphics.Mesh.CMeshDataProvider.MakeBox(-0.5f, -0.5f, -0.5f, 1, 1, 1);
            var rectMesh = rect.ToMesh();
            var ok = mesh.Initialize(rectMesh, materials, Rtti.UTypeDescGetter<Graphics.Mesh.UMdfStaticMesh>.TypeDesc);
            if (ok)
            {
                mesh.SetWorldMatrix(ref Matrix.mIdentity);
                RenderPolicy.VisibleMeshes.Add(mesh);
            }

            //this.RenderPolicy.GBuffers.SunLightColor = new Vector3(1, 1, 1);
            //this.RenderPolicy.GBuffers.SunLightDirection = new Vector3(1, 1, 1);
            //this.RenderPolicy.GBuffers.SkyLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            //this.RenderPolicy.GBuffers.GroundLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            //this.RenderPolicy.GBuffers.UpdateViewportCBuffer();
        }
        protected override void OnClientChanged(bool bSizeChanged)
        {
            var vpSize = this.ClientSize;
            if (bSizeChanged)
            {
                RenderPolicy?.OnResize(vpSize.X, vpSize.Y);
                if (SnapshotPtr != IntPtr.Zero)
                {
                    var handle = System.Runtime.InteropServices.GCHandle.FromIntPtr(SnapshotPtr);
                    handle.Free();
                }
                SnapshotPtr = System.Runtime.InteropServices.GCHandle.ToIntPtr(System.Runtime.InteropServices.GCHandle.Alloc(RenderPolicy.GetFinalShowRSV()));
            }
        }
        protected override IntPtr GetShowTexture()
        {
            return SnapshotPtr;
        }
        #region CameraControl
        Vector2 mPreMousePt;
        public float CameraMoveSpeed { get; set; } = 1.0f;
        public float CameraMouseWheelSpeed { get; set; } = 1.0f;
        public unsafe override bool OnEvent(ref SDL.SDL_Event e)
        {
            if (this.IsFocused == false)
            {
                return true;
            }
            var keyboards = UEngine.Instance.EventProcessorManager.Keyboards;
            if (e.type == SDL.SDL_EventType.SDL_MOUSEMOTION)
            {
                if (e.button.button == SDL.SDL_BUTTON_LEFT)
                {
                    if (keyboards[(int)SDL.SDL_Scancode.SDL_SCANCODE_LALT])
                    {
                        CameraController.Rotate(Graphics.Pipeline.ECameraAxis.Up, (e.motion.x - mPreMousePt.X) * 0.01f);
                        CameraController.Rotate(Graphics.Pipeline.ECameraAxis.Right, (e.motion.y - mPreMousePt.Y) * 0.01f);
                    }
                }
                else if (e.button.button == SDL.SDL_BUTTON_MIDDLE)
                {
                    CameraController.Move(Graphics.Pipeline.ECameraAxis.Right, (e.motion.x - mPreMousePt.X) * 0.01f);
                    CameraController.Move(Graphics.Pipeline.ECameraAxis.Up, (e.motion.y - mPreMousePt.Y) * 0.01f);
                }
                else if (e.button.button == SDL.SDL_BUTTON_X1)
                {
                    if (keyboards[(int)SDL.SDL_Scancode.SDL_SCANCODE_LALT])
                    {
                        CameraController.Move(Graphics.Pipeline.ECameraAxis.Forward, (e.motion.y - mPreMousePt.Y) * 0.03f);
                    }
                    else
                    {
                        CameraController.Rotate(Graphics.Pipeline.ECameraAxis.Up, (e.motion.x - mPreMousePt.X) * 0.01f, true);
                        CameraController.Rotate(Graphics.Pipeline.ECameraAxis.Right, (e.motion.y - mPreMousePt.Y) * 0.01f, true);
                    }
                }

                mPreMousePt.X = e.motion.x;
                mPreMousePt.Y = e.motion.y;
            }
            else if (e.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
            {
                if (keyboards[(int)SDL.SDL_Scancode.SDL_SCANCODE_LALT])
                {
                    CameraMoveSpeed += (float)(e.wheel.y * 0.01f);
                }
                else
                {
                    CameraController.Move(Graphics.Pipeline.ECameraAxis.Forward, e.wheel.y * CameraMouseWheelSpeed);
                }
            }
            return true;
        }
        #endregion
        public void TickLogic(int ellapse)
        {
            if (IsDrawing == false)
                return;
            if (this.IsFocused)
            {
                var keyboards = UEngine.Instance.EventProcessorManager.Keyboards;
                float step = (UEngine.Instance.ElapseTickCount * 0.001f) * CameraMoveSpeed;
                if (keyboards[(int)SDL.SDL_Scancode.SDL_SCANCODE_W])
                {
                    CameraController.Move(Graphics.Pipeline.ECameraAxis.Forward, step);
                }
                else if (keyboards[(int)SDL.SDL_Scancode.SDL_SCANCODE_S])
                {
                    CameraController.Move(Graphics.Pipeline.ECameraAxis.Forward, -step);
                }

                if (keyboards[(int)SDL.SDL_Scancode.SDL_SCANCODE_A])
                {
                    CameraController.Move(Graphics.Pipeline.ECameraAxis.Right, step);
                }
                else if (keyboards[(int)SDL.SDL_Scancode.SDL_SCANCODE_D])
                {
                    CameraController.Move(Graphics.Pipeline.ECameraAxis.Right, -step);
                }
            }

            RenderPolicy?.TickLogic();
        }
        public void TickRender(int ellapse)
        {
            if (IsDrawing == false)
                return;
            RenderPolicy?.TickRender();
        }
        public void TickSync(int ellapse)
        {
            if (IsDrawing == false)
                return;
            RenderPolicy?.TickSync();
        }
    }
}