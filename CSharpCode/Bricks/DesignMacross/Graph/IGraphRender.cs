﻿using EngineNS.DesignMacross.Editor;
using EngineNS.DesignMacross.Render;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngineNS.DesignMacross.Graph
{

    public struct FGraphRenderingContext
    {
        public TtCommandHistory CommandHistory { get; set; }
        public TtEditorInteroperation EditorInteroperation { get; set; }
        public TtGraphViewPort ViewPort { get; set; }
        public TtGraphCamera Camera { get; set; }
        public Vector2 ViewPortTransform(Vector2 pos)
        {
            return ViewPort.ViewPortTransform(Camera.Location, pos);
        }
        public Vector2 ViewPortInverseTransform(Vector2 pos)
        {
            return ViewPort.ViewPortInverseTransform(Camera.Location, pos);
        }
    }
    public struct FGraphElementRenderingContext
    {
        public TtCommandHistory CommandHistory { get; set; }
        public TtEditorInteroperation EditorInteroperation { get; set; }
        public TtGraphViewPort ViewPort { get; set; }
        public TtGraphCamera Camera { get; set; }
        public Vector2 ViewPortTransform(Vector2 pos)
        {
            return ViewPort.ViewPortTransform(Camera.Location, pos);
        }
        public Vector2 ViewPortInverseTransform(Vector2 pos)
        {
            return ViewPort.ViewPortInverseTransform(Camera.Location, pos);
        }
    }
    public interface IGraphElementRender : IElementRender<FGraphElementRenderingContext>
    {
        //public void Draw(ref FGraphElementRenderingContext context);
    }

    public interface IGraphRender : IElementRender<FGraphRenderingContext>
    {
        //public IGraph Graph { get; set; }
        //public void Draw(ref FGraphRenderingContext context);
    }
}
