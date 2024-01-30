using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

namespace Trackman
{
    [DebuggerStepThrough]
    public static class UIElementsExtensions
    {
        #region Methods
        public static void SetMargin(this IStyle style, float margin) => style.marginBottom = style.marginLeft = style.marginRight = style.marginTop = margin;
        public static void SetMarginVertical(this IStyle style, float margin) => style.marginLeft = style.marginRight = margin;
        public static void SetMarginHorizontal(this IStyle style, float margin) => style.marginLeft = style.marginRight = margin;
        public static void SetPadding(this IStyle style, float padding) => style.paddingBottom = style.paddingLeft = style.paddingRight = style.paddingTop = padding;
        public static void SetPaddingVertical(this IStyle style, float padding) => style.paddingBottom = style.paddingTop = padding;
        public static void SetPaddingHorizontal(this IStyle style, float padding) => style.paddingLeft = style.paddingRight = padding;
        public static void SetBorderWidth(this IStyle style, float width) => style.borderBottomWidth = style.borderLeftWidth = style.borderRightWidth = style.borderTopWidth = width;
        public static void SetBorderColor(this IStyle style, Color color) => style.borderBottomColor = style.borderLeftColor = style.borderRightColor = style.borderTopColor = new StyleColor(color);
        public static void SetBorderRadius(this IStyle style, float radius) => style.borderBottomLeftRadius = style.borderBottomRightRadius = style.borderTopLeftRadius = style.borderTopRightRadius = radius;
        #endregion
    }
}