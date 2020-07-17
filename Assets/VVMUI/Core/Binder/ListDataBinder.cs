using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder
{
    [ExecuteInEditMode]
    public class ListDataBinder : AbstractDataBinder
    {
        public DataDefiner Source;
        public GameObject Template;

        public bool Optimize = false;
        public Canvas Canvas = null;
        public RectTransform ViewPort = null;
        public ScrollRect ScrollRect = null;
        public LayoutGroup LayoutGroup = null;
        public int PageItemsCount = 0;

        private IListData sourceData;
        private VMBehaviour vm;

        private int itemsCount = 0;
        private int startIndex = -1;
        private int endIndex = -1;
        private RectOffset originPadding;

        private bool dirty = false;
        private bool resetArrangeIndex = false;

#if UNITY_EDITOR
        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            if (Canvas == null)
            {
                Canvas = this.transform.GetComponentInParent<Canvas>();
            }
            if (ViewPort == null && this.transform.parent != null)
            {
                ViewPort = this.transform.parent as RectTransform;
            }
            if (ScrollRect == null && ViewPort != null && ViewPort.GetComponentInParent<ScrollRect>() != null)
            {
                ScrollRect = ViewPort.GetComponentInParent<ScrollRect>();
            }
            if (LayoutGroup == null)
            {
                LayoutGroup = this.GetComponent<LayoutGroup>();
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            this.CalculatePageItemsCount();
        }
#endif

        private void CalculatePageItemsCount()
        {
            if (this.Template != null && this.IsOptimizeLayout())
            {
                RectTransform templateRectTransform = this.Template.transform as RectTransform;
                HorizontalLayoutGroup hLayout = this.LayoutGroup as HorizontalLayoutGroup;
                VerticalLayoutGroup vLayout = this.LayoutGroup as VerticalLayoutGroup;
                GridLayoutGroup gLayout = this.LayoutGroup as GridLayoutGroup;
                if (this.ViewPort.rect.width <= 1 || this.ViewPort.rect.height <= 1)
                {
                    return;
                }

                if (hLayout != null)
                {
                    this.PageItemsCount = Mathf.CeilToInt(this.ViewPort.rect.width / (templateRectTransform.rect.width + hLayout.spacing));
                }
                if (vLayout != null)
                {
                    this.PageItemsCount = Mathf.CeilToInt(this.ViewPort.rect.height / (templateRectTransform.rect.height + vLayout.spacing));
                }
                if (gLayout != null)
                {
                    int rowsCount = Mathf.FloorToInt(this.ViewPort.rect.height / (templateRectTransform.rect.height + gLayout.spacing.y));
                    int collumnsCount = Mathf.FloorToInt(this.ViewPort.rect.width / (templateRectTransform.rect.width + gLayout.spacing.x));
                    switch (gLayout.constraint)
                    {
                        case GridLayoutGroup.Constraint.FixedRowCount:
                            rowsCount = gLayout.constraintCount;
                            break;
                        case GridLayoutGroup.Constraint.FixedColumnCount:
                            collumnsCount = gLayout.constraintCount;
                            break;
                    }
                    this.PageItemsCount = rowsCount * collumnsCount;
                }
            }
        }

        private bool IsOptimizeLayout()
        {
            return this.Optimize && this.Template != null && this.ScrollRect != null && this.ViewPort != null && this.LayoutGroup != null && this.Canvas != null;
        }

        protected override void Awake()
        {
            base.Awake();

            if (!IsOptimizeLayout())
            {
                return;
            }

            this.originPadding = this.LayoutGroup.padding;

            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }

            this.ScrollRect.onValueChanged.AddListener(delegate (Vector2 pos)
            {
                this.CalculatePageItemsCount();
                this.CalculateDisplayRange();
            });
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // 组件激活时需要设置一次数据
            this.SetDirty(true);
        }

        private static Rect GetRectOfTransformInCanvas(RectTransform transform, Canvas canvas)
        {
            Vector2[] cornersInCanvans = new Vector2[4];
            Vector3[] corners = new Vector3[4];
            transform.GetWorldCorners(corners);

            Camera camera = canvas.worldCamera;
            for (int i = 0; i < corners.Length; i++)
            {
                if (camera != null)
                {
                    corners[i] = camera.WorldToScreenPoint(corners[i]);
                }
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, corners[i], canvas.worldCamera, out pos);
                cornersInCanvans[i] = pos;
            }

            Rect rect = new Rect(cornersInCanvans[0], new Vector2(cornersInCanvans[2].x - cornersInCanvans[1].x, cornersInCanvans[1].y - cornersInCanvans[0].y));
            return rect;
        }

        private void CalculateDisplayRange()
        {
            if (this.sourceData == null || !this.IsOptimizeLayout() || this.transform.childCount <= 0)
            {
                return;
            }

            if (this.dirty)
            {
                return;
            }

            // RectTransform.rect 不是准确的区域，自行实现通过四个角的坐标换算出真实的区域

            Rect viewportRect, firstChildRect, lastChildRect;
            // viewportRect = new Rect (ViewPort.rect.x + ViewPort.position.x, ViewPort.rect.y + ViewPort.position.y, ViewPort.rect.width, ViewPort.rect.height);
            viewportRect = GetRectOfTransformInCanvas(ViewPort, Canvas);

            RectTransform firstChild = this.transform.GetChild(0) as RectTransform;
            // firstChildRect = new Rect (firstChild.rect.x + firstChild.position.x, firstChild.rect.y + firstChild.position.y, firstChild.rect.width, firstChild.rect.height);
            firstChildRect = GetRectOfTransformInCanvas(firstChild, Canvas);

            RectTransform lastChild = this.transform.GetChild(this.transform.childCount - 1) as RectTransform;
            // lastChildRect = new Rect (lastChild.rect.x + lastChild.position.x, lastChild.rect.y + lastChild.position.y, lastChild.rect.width, lastChild.rect.height);
            lastChildRect = GetRectOfTransformInCanvas(lastChild, Canvas);

            int stepCount = PageItemsCount / 2;

            HorizontalLayoutGroup hLayout = this.LayoutGroup as HorizontalLayoutGroup;
            VerticalLayoutGroup vLayout = this.LayoutGroup as VerticalLayoutGroup;
            GridLayoutGroup gLayout = this.LayoutGroup as GridLayoutGroup;

            bool prevNextPage = false;
            bool sufNextPage = false;
            if (hLayout != null || (gLayout != null && gLayout.startAxis == GridLayoutGroup.Axis.Vertical))
            {
                prevNextPage = firstChildRect.center.x > viewportRect.xMin;
                sufNextPage = lastChildRect.center.x < viewportRect.xMax;
            }
            if (vLayout != null || (gLayout != null && gLayout.startAxis == GridLayoutGroup.Axis.Horizontal))
            {
                prevNextPage = firstChildRect.center.y < viewportRect.yMax;
                sufNextPage = lastChildRect.center.y > viewportRect.yMin;
            }

            if (prevNextPage && this.startIndex > 0)
            {
                this.startIndex -= stepCount;
                this.endIndex = this.startIndex + this.PageItemsCount * 2;
                this.SetDirty();
                return;
            }

            if (sufNextPage && this.endIndex < this.sourceData.Count)
            {
                this.endIndex += stepCount;
                this.startIndex = this.endIndex - this.PageItemsCount * 2;
                this.SetDirty();
                return;
            }
        }

        public override void Bind(VMBehaviour vm)
        {
            if (Source == null)
            {
                return;
            }

            IData data = Source.GetData(vm);
            if (data == null)
            {
                return;
            }

            this.sourceData = data as IListData;
            if (this.sourceData == null)
            {
                return;
            }

            this.vm = vm;

            this.DoBind();
        }

        public override void Bind(VMBehaviour vm, IData data)
        {
            if (Source == null)
            {
                return;
            }

            if (data == null)
            {
                return;
            }

            this.sourceData = Source.GetData(data) as IListData;
            if (this.sourceData == null)
            {
                return;
            }

            this.vm = vm;

            this.DoBind();
        }

        public override void UnBind()
        {
            this.itemsCount = 0;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }

            if (this.sourceData != null)
            {
                (this.sourceData as IData).RemoveValueChangedListener(SetDirty);
                this.sourceData.FocusIndexChanged -= FocusIndexChanged;
                this.sourceData = null;
                this.vm = null;
            }
        }

        private void DoBind()
        {
            this.itemsCount = 0;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }

            (this.sourceData as IData).AddValueChangedListener(SetDirty);
            this.sourceData.FocusIndexChanged += FocusIndexChanged;

            this.SetDirty(true);
        }

        private void FocusIndexChanged()
        {
            this.SetDirty(true);
        }

        private void SetDirty()
        {
            this.SetDirty(false);
        }

        private void SetDirty(bool resetIndex)
        {
            if (resetIndex)
            {
                if (this.IsOptimizeLayout() && this.sourceData != null)
                {
                    if (this.sourceData.FocusIndex < this.PageItemsCount)
                    {
                        this.startIndex = 0;
                        this.endIndex = this.PageItemsCount * 2;
                    }
                    else if (this.sourceData.FocusIndex >= this.sourceData.Count - this.PageItemsCount)
                    {
                        this.startIndex = this.sourceData.Count - this.PageItemsCount * 2;
                        this.endIndex = this.sourceData.Count;
                    }
                    else
                    {
                        this.startIndex = this.sourceData.FocusIndex - this.PageItemsCount;
                        this.endIndex = this.sourceData.FocusIndex + this.PageItemsCount;
                    }
                }
                else
                {
                    this.startIndex = 0;
                    this.endIndex = -1;
                }
            }

            if (this.dirty)
            {
                return;
            }

            if (this.isActiveAndEnabled)
            {
                this.dirty = true;
                StartCoroutine(DelayArrange(resetIndex));
            }
        }

        private IEnumerator DelayArrange(bool focus)
        {
            yield return null;
            Arrange(focus);

            if (focus)
            {
                yield return null;
                SetFocus();
            }

            this.dirty = false;
        }

        private void Arrange(bool focus)
        {
            if (Template == null || this.sourceData == null)
            {
                return;
            }

            int start = Mathf.Max(0, startIndex);
            int end = endIndex >= 0 ? Mathf.Min(endIndex, this.sourceData.Count) : this.sourceData.Count;

            int dataCount = end - start;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                ListTemplateBinder binder = child.GetComponent<ListTemplateBinder>();
                if (binder != null && i < dataCount)
                {
                    binder.ReBind(start + i);
                }
            }

            if (itemsCount < dataCount)
            {
                while (itemsCount < dataCount)
                {
                    GameObject obj = GameObject.Instantiate(Template, this.transform);
                    obj.SetActive(true);
                    ListTemplateBinder binder = obj.GetComponent<ListTemplateBinder>();
                    if (binder != null)
                    {
                        binder.Bind(this.vm, start + itemsCount, this.sourceData);
                        this.itemsCount++;
                    }
                }
            }
            else if (itemsCount > dataCount)
            {
                for (int i = 0; i < this.transform.childCount; i++)
                {
                    GameObject obj = this.transform.GetChild(i).gameObject;
                    ListTemplateBinder binder = obj.GetComponent<ListTemplateBinder>();
                    if (binder != null)
                    {
                        if (i >= dataCount)
                        {
                            binder.UnBind();
                            GameObject.Destroy(obj);
                            this.itemsCount--;
                        }
                    }
                }
            }

            if (this.IsOptimizeLayout())
            {
                this.SetPadding();
            }
        }

        private void SetFocus()
        {
            RectTransform templateRectTransform = this.Template.transform as RectTransform;
            if (templateRectTransform == null)
            {
                return;
            }

            RectTransform rectTransform = this.transform as RectTransform;
            HorizontalLayoutGroup hLayout = this.LayoutGroup as HorizontalLayoutGroup;
            VerticalLayoutGroup vLayout = this.LayoutGroup as VerticalLayoutGroup;
            GridLayoutGroup gLayout = this.LayoutGroup as GridLayoutGroup;
            Vector2 originPosition = rectTransform.anchoredPosition;

            if (hLayout != null)
            {
                float x = -this.sourceData.FocusIndex * (templateRectTransform.rect.width + hLayout.spacing);
                rectTransform.anchoredPosition = new Vector2(x, originPosition.y);
            }
            if (vLayout != null)
            {
                float y = this.sourceData.FocusIndex * (templateRectTransform.rect.height + vLayout.spacing);
                rectTransform.anchoredPosition = new Vector2(originPosition.x, y);
            }
        }

        private void SetPadding()
        {
            if (!this.IsOptimizeLayout())
            {
                return;
            }

            RectTransform templateRectTransform = this.Template.transform as RectTransform;
            if (templateRectTransform == null)
            {
                return;
            }

            HorizontalLayoutGroup hLayout = this.LayoutGroup as HorizontalLayoutGroup;
            VerticalLayoutGroup vLayout = this.LayoutGroup as VerticalLayoutGroup;
            GridLayoutGroup gLayout = this.LayoutGroup as GridLayoutGroup;

            float startWidth = 0;
            float startHeight = 0;
            float endWidth = 0;
            float endHeight = 0;
            int startCount = Mathf.Clamp(startIndex, 0, this.sourceData.Count);
            int endCount = Mathf.Clamp(this.sourceData.Count - endIndex, 0, this.sourceData.Count);

            if (hLayout != null)
            {
                startWidth = startCount * templateRectTransform.rect.width + (startCount >= 1 ? (startCount - 1) : 0) * hLayout.spacing;
                endWidth = endCount * templateRectTransform.rect.width + (endCount >= 1 ? (endCount - 1) : 0) * hLayout.spacing;
                startHeight = templateRectTransform.rect.height;
                endHeight = templateRectTransform.rect.height;
                hLayout.padding = new RectOffset(this.originPadding.left + (int)startWidth, this.originPadding.right + (int)endWidth, this.originPadding.top, this.originPadding.bottom);
            }
            else if (vLayout != null)
            {
                startWidth = templateRectTransform.rect.width;
                endWidth = templateRectTransform.rect.width;
                startHeight = startCount * templateRectTransform.rect.height + (startCount >= 1 ? (startCount - 1) : 0) * vLayout.spacing;
                endHeight = endCount * templateRectTransform.rect.height + (endCount >= 1 ? (endCount - 1) : 0) * vLayout.spacing;
                vLayout.padding = new RectOffset(this.originPadding.left, this.originPadding.right, this.originPadding.top + (int)startHeight, this.originPadding.bottom + (int)endHeight);
            }
            else if (gLayout != null)
            {
                int rowsCount = Mathf.FloorToInt(this.ViewPort.rect.height / templateRectTransform.rect.height);
                int collumnsCount = Mathf.FloorToInt(this.ViewPort.rect.width / templateRectTransform.rect.width);
                switch (gLayout.constraint)
                {
                    case GridLayoutGroup.Constraint.FixedRowCount:
                        rowsCount = gLayout.constraintCount;
                        break;
                    case GridLayoutGroup.Constraint.FixedColumnCount:
                        collumnsCount = gLayout.constraintCount;
                        break;
                }
                if (gLayout.startAxis == GridLayoutGroup.Axis.Horizontal)
                {
                    startCount = Mathf.CeilToInt(startCount / collumnsCount);
                    endCount = Mathf.CeilToInt(endCount / collumnsCount);
                    startWidth = this.ViewPort.rect.width;
                    endWidth = this.ViewPort.rect.width;
                    startHeight = startCount * templateRectTransform.rect.height + (startCount >= 1 ? (startCount - 1) : 0) * gLayout.spacing.y;
                    endHeight = endCount * templateRectTransform.rect.height + (endCount >= 1 ? (endCount - 1) : 0) * gLayout.spacing.y;
                    gLayout.padding = new RectOffset(this.originPadding.left, this.originPadding.right, this.originPadding.top + (int)startHeight, this.originPadding.bottom + (int)endHeight);
                }
                else
                {
                    startCount = Mathf.CeilToInt(startCount / rowsCount);
                    endCount = Mathf.CeilToInt(endCount / rowsCount);
                    startWidth = startCount * templateRectTransform.rect.width + (startCount >= 1 ? (startCount - 1) : 0) * gLayout.spacing.x;
                    endWidth = endCount * templateRectTransform.rect.width + (endCount >= 1 ? (endCount - 1) : 0) * gLayout.spacing.x;
                    startHeight = this.ViewPort.rect.height;
                    endHeight = this.ViewPort.rect.height;
                    gLayout.padding = new RectOffset(this.originPadding.left + (int)startWidth, this.originPadding.right + (int)endWidth, this.originPadding.top, this.originPadding.bottom);
                }
            }
        }
    }
}