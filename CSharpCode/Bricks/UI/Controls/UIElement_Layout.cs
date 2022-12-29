﻿using EngineNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using System.Text;

namespace EngineNS.UI.Controls
{
    public partial class UIElement
    {
        [Flags]
        internal enum eLayoutFlags : uint
        {
            None = 0,
            MeasureDirty = 1 << 0,
            ArrangeDirty = 1 << 1,
            MeasureInProgress = 1 << 2,
            ArrangeInProgress = 1 << 3,
            NeverMeasured = 1 << 4,
            NeverArranged = 1 << 5,
            MeasureDuringArrange = 1 << 6,
            IsLayoutIslandRoot = 1 << 7,
        }
        private eLayoutFlags mLayoutFlags;
        internal bool ReadFlag(eLayoutFlags flag)
        {
            return (mLayoutFlags & flag) != 0;
        }
        internal void WriteFlag(eLayoutFlags flag, bool value)
        {
            if (value)
                mLayoutFlags |= flag;
            else
                mLayoutFlags &= ~flag;
        }
        internal bool IsLayoutIslandRoot
        {
            get { return ReadFlag(eLayoutFlags.IsLayoutIslandRoot); }
            set { WriteFlag(eLayoutFlags.IsLayoutIslandRoot, value); }
        }
        internal bool MeasureDirty
        {
            get { return ReadFlag(eLayoutFlags.MeasureDirty); }
            set { WriteFlag(eLayoutFlags.MeasureDirty, value); }
        }
        internal bool ArrangeDirty
        {
            get { return ReadFlag(eLayoutFlags.ArrangeDirty); }
            set { WriteFlag(eLayoutFlags.ArrangeDirty, value); }
        }
        internal bool MeasureInProgress
        {
            get { return ReadFlag(eLayoutFlags.MeasureInProgress); }
            set { WriteFlag(eLayoutFlags.MeasureInProgress, value); }
        }
        internal bool ArrangeInProgress
        {
            get { return ReadFlag(eLayoutFlags.ArrangeInProgress); }
            set { WriteFlag(eLayoutFlags.ArrangeInProgress, value); }
        }
        internal bool NeverMeasured
        {
            get { return ReadFlag(eLayoutFlags.NeverMeasured); }
            set { WriteFlag(eLayoutFlags.NeverMeasured, value); }
        }
        internal bool NeverArranged
        {
            get { return ReadFlag(eLayoutFlags.NeverArranged); }
            set { WriteFlag(eLayoutFlags.NeverArranged, value); }
        }
        internal bool MeasureDuringArrange
        {
            get { return ReadFlag(eLayoutFlags.MeasureDuringArrange); }
            set { WriteFlag(eLayoutFlags.MeasureDuringArrange, value); }
        }
        [Browsable(false)]
        public bool IsArrangeValid { get => !ArrangeDirty; }
        [Browsable(false)]
        public bool IsMeasureValid { get => !MeasureDirty; }
        internal void InvalidateMeasureInternal() { MeasureDirty = true; }
        internal void InvalidateArrangeInternal() { ArrangeDirty = true; }

        internal Layout.LayoutQueue.Request MeasureRequest;
        internal Layout.LayoutQueue.Request ArrangeRequest;

        internal virtual UInt32 TreeLevel
        {
            get;
            set;
        } = 0;

        public virtual void UpdateLayout()
        {
            if (Parent == null)
                return;

            InvalidateMeasure();
        }
        public UIElement GetUIParentWithinLayoutIsland()
        {
            var uiParent = this.Parent;
            if (uiParent != null && uiParent.IsLayoutIslandRoot)
                return null;
            return uiParent;
        }
        protected void InvalidateMeasure()
        {
            UEngine.Instance.EventPoster.RunOn(() =>
            {
                if (!MeasureDirty && !MeasureInProgress)
                {
                    System.Diagnostics.Debug.Assert(MeasureRequest == null, "can't be clean and still have MeasureRequest");
                    EngineNS.UEngine.Instance.UILayoutManager.MeasureQueue.Add(this);
                    MeasureDirty = true;
                }
                return true;
            }, Thread.Async.EAsyncTarget.Logic);
        }
        protected void InvalidateArrange()
        {
            UEngine.Instance.EventPoster.RunOn(() =>
            {
                if (!ArrangeDirty && !ArrangeInProgress)
                {
                    if (Parent == null || !Parent.ArrangeDirty)
                    {
                        System.Diagnostics.Debug.Assert(ArrangeRequest == null, "can't be clean and still have MeasureRequest");
                        UEngine.Instance.UILayoutManager.ArrangeQueue.Add(this);
                    }
                    ArrangeDirty = true;
                }
                return true;
            }, Thread.Async.EAsyncTarget.Logic);
        }
        internal virtual void MarkTreeDirty()
        {
            InvalidateMeasureInternal();
            InvalidateArrangeInternal();
        }
        EngineNS.SizeF mPreviousAvailableSize = EngineNS.SizeF.Infinity;
        public void ResetPreviousAvailableSize()
        {
            mPreviousAvailableSize = EngineNS.SizeF.Infinity;
        }
        internal EngineNS.SizeF PreviousAvailableSize => mPreviousAvailableSize;
        private EngineNS.SizeF mDesiredSize;
        internal EngineNS.SizeF DesiredSize
        {
            get
            {
                if (this.Visibility == Visibility.Collapsed)
                    return new SizeF(0, 0);
                else
                    return mDesiredSize;
            }
            set
            {
                mDesiredSize = value;
            }
        }
        EngineNS.SizeF mPrevDesiredSize;
        internal void Measure(ref EngineNS.SizeF availableSize)
        {
            try
            {
                if (FloatUtil.IsNaN(availableSize.Width) || FloatUtil.IsNaN(availableSize.Height))
                    throw new InvalidOperationException("Measure exception availableSize is NaN");

                var neverMeasured = NeverMeasured;
                if (neverMeasured)
                {

                }

                var isCloseToPreviousMeasure = mPreviousAvailableSize.Equals(ref availableSize);
                if (this.Visibility == Visibility.Collapsed)
                {
                    if (MeasureRequest != null)
                        EngineNS.UEngine.Instance.UILayoutManager.MeasureQueue.Remove(this);
                    if (isCloseToPreviousMeasure)
                        MeasureDirty = false;
                    else
                    {
                        InvalidateMeasureInternal();
                        mPreviousAvailableSize = availableSize;
                    }
                    return;
                }

                if (IsMeasureValid && !neverMeasured && isCloseToPreviousMeasure)
                    return;

                NeverMeasured = false;
                mPrevDesiredSize = mDesiredSize;
                InvalidateArrange();
                MeasureInProgress = true;
                var desiredSize = new EngineNS.SizeF(0, 0);
                bool gotException = true;
                try
                {
                    EngineNS.UEngine.Instance.UILayoutManager.EnterMeasure();
                    desiredSize = MeasureCore(ref availableSize);
                    gotException = false;
                }
                finally
                {
                    MeasureInProgress = false;
                    mPreviousAvailableSize = availableSize;
                    EngineNS.UEngine.Instance.UILayoutManager.ExitMeasure();
                    if (gotException)
                    {
                        if (EngineNS.UEngine.Instance.UILayoutManager.LastExceptionElement == null)
                            EngineNS.UEngine.Instance.UILayoutManager.LastExceptionElement = this;
                    }
                }

                // desiredSize必须为有效值
                if (float.IsPositiveInfinity(desiredSize.Width) || float.IsPositiveInfinity(desiredSize.Height))
                    throw new InvalidOperationException("Measure Exception: desiredSize IsPositiveInfinity");
                if (FloatUtil.IsNaN(desiredSize.Width) || FloatUtil.IsNaN(desiredSize.Height))
                    throw new InvalidOperationException("Measure Exception: desiredSize IsNaN");

                MeasureDirty = false;
                if (MeasureRequest != null)
                    EngineNS.UEngine.Instance.UILayoutManager.MeasureQueue.Remove(this);

                mDesiredSize = desiredSize;
                if (!MeasureDuringArrange && !mPrevDesiredSize.Equals(ref desiredSize))
                {
                    var p = this.Parent;
                    if (p != null && !p.MeasureInProgress)
                        p.OnChildDesiredSizeChanged(this);
                }
            }
            catch (System.Exception e)
            {
                EngineNS.Profiler.Log.WriteException(e);
            }
        }
        public Controls.Containers.UIContainerSlot Slot { get; set; }
        public virtual void OnChildDesiredSizeChanged(UIElement child)
        {
            if (IsMeasureValid && Slot != null)
            {
                if (Slot.NeedUpdateLayoutWhenChildDesiredSizeChanged(child))
                    InvalidateMeasure();
            }
        }
        protected virtual EngineNS.SizeF MeasureCore(ref EngineNS.SizeF availableSize)
        {
            var frameworkAvailableSize = new EngineNS.SizeF(Math.Max(availableSize.Width, 0), Math.Max(availableSize.Height, 0));
            var desiredSize = frameworkAvailableSize;
            if (Slot != null)
            {
                desiredSize = Slot.Measure(ref frameworkAvailableSize);
            }
            else
            {
                desiredSize = MeasureOverride(ref frameworkAvailableSize);
            }

            return desiredSize;
        }

        public virtual EngineNS.SizeF MeasureOverride(ref EngineNS.SizeF availableSize)
        {
            return availableSize;
        }

        EngineNS.RectangleF mCurFinalRect;
        internal EngineNS.RectangleF PreviousArrangeRect => mCurFinalRect;
        public void Arrange(ref EngineNS.RectangleF finalRect)
        {
            try
            {
                if (float.IsPositiveInfinity(finalRect.Width) || float.IsPositiveInfinity(finalRect.Height) || FloatUtil.IsNaN(finalRect.Width) || FloatUtil.IsNaN(finalRect.Height))
                    throw new InvalidOperationException("Arrange Exception: finalRect illegal!");

                if (this.Visibility == Visibility.Collapsed)
                {
                    if (ArrangeRequest != null)
                        EngineNS.UEngine.Instance.UILayoutManager.ArrangeQueue.Remove(this);
                    mCurFinalRect = finalRect;
                    ArrangeDirty = false;
                    return;
                }

                if (MeasureDirty || NeverMeasured)
                {
                    try
                    {
                        MeasureDuringArrange = true;
                        if (NeverMeasured)
                        {
                            var size = finalRect.Size;
                            Measure(ref size);
                        }
                        else
                            Measure(ref mPreviousAvailableSize);
                    }
                    finally
                    {
                        MeasureDuringArrange = false;
                    }
                }

                if (!IsArrangeValid || NeverArranged || !(finalRect.Equals(ref mCurFinalRect) && mPrevDesiredSize.Equals(ref mDesiredSize)))
                {
                    NeverArranged = false;
                    ArrangeInProgress = true;

                    var oldSize = DesignRect.Size;
                    bool gotException = true;
                    try
                    {
                        EngineNS.UEngine.Instance.UILayoutManager.EnterArrange();
                        ArrangeCore(ref finalRect);
                        gotException = false;
                    }
                    finally
                    {
                        ArrangeInProgress = false;
                        EngineNS.UEngine.Instance.UILayoutManager.ExitArrange();
                        if (gotException)
                        {
                            if (EngineNS.UEngine.Instance.UILayoutManager.LastExceptionElement == null)
                                EngineNS.UEngine.Instance.UILayoutManager.LastExceptionElement = this;
                        }
                    }

                    mCurFinalRect = finalRect;
                    ArrangeDirty = false;
                    if (ArrangeRequest != null)
                        EngineNS.UEngine.Instance.UILayoutManager.ArrangeQueue.Remove(this);
                }
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(this.GetType().ToString() + " Arrange Exception: \r\n" + e.ToString());
            }
            finally
            {

            }
        }
        protected virtual void ArrangeCore(ref EngineNS.RectangleF finalRect)
        {
            var arrangeSize = finalRect;

            var oldRect = DesignRect;
            if (Slot != null)
                Slot.Arrange(ref arrangeSize);
            else
                ArrangeOverride(ref arrangeSize);

            if (!DesignRect.Equals(ref oldRect))
            {
                UpdateDesignClipRect();
            }
        }
        public virtual void ArrangeOverride(ref EngineNS.RectangleF arrangeSize)
        {
            if (mDesignRect.Equals(ref arrangeSize))
                return;
            DesignRect = arrangeSize;
            UpdateDesignClipRect();
        }

    }
}