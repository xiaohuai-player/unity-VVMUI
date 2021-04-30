using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder
{
    [ExecuteInEditMode]
    public class ListDataBinder : AbstractDataBinder
    {
        private static Vector2[] cornersInCanvans = new Vector2[4];
        private static Vector3[] corners = new Vector3[4];
        private static Rect GetRectOfTransformInCanvas(RectTransform transform, Canvas canvas)
        {
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

        public DataDefiner Source;
        public GameObject Template;

        public bool Optimize = true;
        public Canvas Canvas = null;
        public RectTransform ViewPort = null;
        public ScrollRect ScrollRect = null;
        public LayoutGroup LayoutGroup = null;
        public int PageRowsCount = 0;
        public int StepRowsCount = 0;
        public int RowItemsCount = 0;

        private IListData sourceData;

        private int itemsCount = 0;
        private int startIndex = -1;
        private int endIndex = -1;
        private int gridRowCount = 0;
        private int gridCollumnCount = 0;
        private RectOffset originPadding;

        private bool dirty = false;

#if UNITY_EDITOR
        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            this.GetComponents();

            if (!this.IsOptimizeLayout())
            {
                return;
            }

            this.CalculatePageItemsCount();
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            this.GetComponents();

            if (!IsOptimizeLayout())
            {
                return;
            }

            this.originPadding = this.LayoutGroup.padding;

            this.CalculatePageItemsCount();
            this.CalculateDisplayRange();

            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }

            this.ScrollRect.onValueChanged.AddListener(delegate (Vector2 pos)
            {
                this.CalculateDisplayRange();
            });
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // 组件激活时需要设置一次数据
            this.SetDirty(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            this.dirty = false;
        }

        private void GetComponents()
        {
            if (Canvas == null)
            {
                Canvas = this.GetComponentInParent<Canvas>(true);
            }
            if (ViewPort == null && this.transform.parent != null)
            {
                ViewPort = this.transform.parent as RectTransform;
            }
            if (ScrollRect == null && ViewPort != null && ViewPort.GetComponentInParent<ScrollRect>(true) != null)
            {
                ScrollRect = ViewPort.GetComponentInParent<ScrollRect>(true);
            }
            if (LayoutGroup == null)
            {
                LayoutGroup = this.GetComponent<LayoutGroup>();
            }
        }

        private bool IsOptimizeLayout()
        {
            return this.Optimize && this.Template != null && this.ScrollRect != null && this.ViewPort != null && this.LayoutGroup != null;
        }

        private void CalculatePageItemsCount()
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
                this.PageRowsCount = Mathf.CeilToInt(this.ViewPort.rect.width / (templateRectTransform.rect.width + hLayout.spacing));
                this.RowItemsCount = 1;
            }
            if (vLayout != null)
            {
                this.PageRowsCount = Mathf.CeilToInt(this.ViewPort.rect.height / (templateRectTransform.rect.height + vLayout.spacing));
                this.RowItemsCount = 1;
            }
            if (gLayout != null)
            {
                float fRowsCount = this.ViewPort.rect.height / (templateRectTransform.rect.height + gLayout.spacing.y);
                float fCollumnsCount = this.ViewPort.rect.width / (templateRectTransform.rect.width + gLayout.spacing.x);
                if (gLayout.startAxis == GridLayoutGroup.Axis.Horizontal)
                {
                    gridRowCount = Mathf.CeilToInt(fRowsCount);
                    gridCollumnCount = Mathf.FloorToInt(fCollumnsCount);
                }
                else
                {
                    gridRowCount = Mathf.FloorToInt(fRowsCount);
                    gridCollumnCount = Mathf.CeilToInt(fCollumnsCount);
                }
                switch (gLayout.constraint)
                {
                    case GridLayoutGroup.Constraint.FixedRowCount:
                        gridRowCount = gLayout.constraintCount;
                        break;
                    case GridLayoutGroup.Constraint.FixedColumnCount:
                        gridCollumnCount = gLayout.constraintCount;
                        break;
                }
                if (gLayout.startAxis == GridLayoutGroup.Axis.Horizontal)
                {
                    this.PageRowsCount = gridRowCount;
                    this.RowItemsCount = gridCollumnCount;
                }
                else
                {
                    this.PageRowsCount = gridCollumnCount;
                    this.RowItemsCount = gridRowCount;
                }
            }
            this.StepRowsCount = Mathf.CeilToInt((float)this.PageRowsCount / 2f);
        }

        private void CalculateDisplayRange()
        {
            if (this.Canvas == null)
            {
                return;
            }

            if (this.sourceData == null || this.transform.childCount <= 0)
            {
                return;
            }

            if (this.dirty)
            {
                return;
            }

            // RectTransform.rect 不是准确的区域，自行实现通过四个角的坐标换算出真实的区域

            Rect viewportRect, firstChildRect, lastChildRect;
            viewportRect = GetRectOfTransformInCanvas(ViewPort, Canvas);
            firstChildRect = GetRectOfTransformInCanvas(this.transform.GetChild(0) as RectTransform, Canvas);
            lastChildRect = GetRectOfTransformInCanvas(this.transform.GetChild(this.transform.childCount - 1) as RectTransform, Canvas);

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
                this.startIndex -= (StepRowsCount * RowItemsCount);
                this.endIndex = this.startIndex + PageRowsCount * 2 * RowItemsCount;
                this.SetDirty();
                return;
            }

            if (sufNextPage && this.endIndex < this.sourceData.Count)
            {
                this.endIndex += (StepRowsCount * RowItemsCount);
                this.startIndex = this.endIndex - PageRowsCount * 2 * RowItemsCount;
                this.SetDirty();
                return;
            }
        }

        private void CalculateResetDisplayRange()
        {
            if (this.IsOptimizeLayout() && this.sourceData != null)
            {
                if (this.sourceData.FocusIndex < this.PageRowsCount)
                {
                    this.startIndex = 0;
                    this.endIndex = PageRowsCount * 2 * RowItemsCount;
                }
                else if (this.sourceData.FocusIndex >= this.sourceData.Count - this.PageRowsCount)
                {
                    this.startIndex = this.sourceData.Count - PageRowsCount * 2 * RowItemsCount;
                    this.endIndex = this.sourceData.Count;
                }
                else
                {
                    int focusRowIndex = this.sourceData.FocusIndex / RowItemsCount;
                    this.startIndex = (focusRowIndex - this.PageRowsCount) * RowItemsCount;
                    this.endIndex = (focusRowIndex + this.PageRowsCount) * RowItemsCount;
                }
            }
            else
            {
                this.startIndex = 0;
                this.endIndex = -1;
            }
        }

        public override void Bind(VMBehaviour vm)
        {
            base.Bind(vm);

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

            this.DoBind();
        }

        public override void Bind(VMBehaviour vm, IData data)
        {
            base.Bind(vm, data);

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

            this.DoBind();
        }

        public override void UnBind()
        {
            base.UnBind();

            this.itemsCount = 0;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }

            if (this.sourceData != null)
            {
                (this.sourceData as IData).RemoveValueChangedListener(this.SourceDataChanged);
                this.sourceData.FocusIndexChanged -= FocusIndexChanged;
                this.sourceData = null;
            }
        }

        private void DoBind()
        {
            this.itemsCount = 0;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }

            (this.sourceData as IData).AddValueChangedListener(this.SourceDataChanged);
            this.sourceData.FocusIndexChanged += FocusIndexChanged;

            this.SetDirty(true);
        }

        private void FocusIndexChanged()
        {
            this.SetDirty(true);
        }

        private void SourceDataChanged(IData source)
        {
            this.SetDirty();
        }

        private void SetDirty()
        {
            this.SetDirty(false);
        }

        private void SetDirty(bool resetIndex)
        {
            if (resetIndex)
            {
                CalculateResetDisplayRange();
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
            yield return new WaitForEndOfFrame();

            Arrange();

            if (focus)
            {
                SetFocus();
            }

            yield return new WaitForEndOfFrame();

            this.dirty = false;
        }

        private void Arrange()
        {
            if (Template == null || this.sourceData == null)
            {
                return;
            }

            int start = Mathf.Max(0, this.startIndex);
            int end = endIndex >= 0 ? Mathf.Min(this.endIndex, this.sourceData.Count) : this.sourceData.Count;
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
                        binder.Bind(this.BindVM, start + itemsCount, this.sourceData);
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
                    if (binder != null && i >= dataCount)
                    {
                        binder.UnBind();
                        GameObject.Destroy(obj);
                        this.itemsCount--;
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
            if (!IsOptimizeLayout())
            {
                return;
            }

            RectTransform templateRectTransform = this.Template.transform as RectTransform;
            if (templateRectTransform == null)
            {
                return;
            }

            if (this.sourceData == null)
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
                hLayout.padding = new RectOffset(this.originPadding.left + (int)startWidth, this.originPadding.right + (int)endWidth, this.originPadding.top, this.originPadding.bottom);
            }
            else if (vLayout != null)
            {
                startHeight = startCount * templateRectTransform.rect.height + (startCount >= 1 ? (startCount - 1) : 0) * vLayout.spacing;
                endHeight = endCount * templateRectTransform.rect.height + (endCount >= 1 ? (endCount - 1) : 0) * vLayout.spacing;
                vLayout.padding = new RectOffset(this.originPadding.left, this.originPadding.right, this.originPadding.top + (int)startHeight, this.originPadding.bottom + (int)endHeight);
            }
            else if (gLayout != null)
            {
                startCount = Mathf.CeilToInt((float)startCount / (float)RowItemsCount);
                endCount = Mathf.CeilToInt((float)endCount / (float)RowItemsCount);
                if (gLayout.startAxis == GridLayoutGroup.Axis.Horizontal)
                {
                    startHeight = startCount * templateRectTransform.rect.height + (startCount >= 1 ? (startCount - 1) : 0) * gLayout.spacing.y;
                    endHeight = endCount * templateRectTransform.rect.height + (endCount >= 1 ? (endCount - 1) : 0) * gLayout.spacing.y;
                    gLayout.padding = new RectOffset(this.originPadding.left, this.originPadding.right, this.originPadding.top + (int)startHeight, this.originPadding.bottom + (int)endHeight);
                }
                else
                {
                    startWidth = startCount * templateRectTransform.rect.width + (startCount >= 1 ? (startCount - 1) : 0) * gLayout.spacing.x;
                    endWidth = endCount * templateRectTransform.rect.width + (endCount >= 1 ? (endCount - 1) : 0) * gLayout.spacing.x;
                    gLayout.padding = new RectOffset(this.originPadding.left + (int)startWidth, this.originPadding.right + (int)endWidth, this.originPadding.top, this.originPadding.bottom);
                }
            }
        }
    }
}