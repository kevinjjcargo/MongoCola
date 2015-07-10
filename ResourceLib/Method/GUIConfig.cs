﻿/*
 * Created by SharpDevelop.
 * User: scs
 * Date: 2015/1/6
 * Time: 10:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Drawing;
using System.Windows.Forms;

namespace ResourceLib.Method
{
    /// <summary>
    ///     Description of UIConfig.
    /// </summary>
    public static class GuiConfig
    {
        /// <summary>
        ///     成功提示色
        /// </summary>
        public static Color SuccessColor = Color.LightGreen;

        /// <summary>
        ///     失败提示色
        /// </summary>
        public static Color FailColor = Color.Pink;

        /// <summary>
        ///     动作提示色
        /// </summary>
        public static Color ActionColor = Color.LightBlue;

        /// <summary>
        ///     警告提示色
        /// </summary>
        public static Color WarningColor = Color.LightYellow;

        /// <summary>
        ///     是否使用默认语言
        /// </summary>
        public static bool IsUseDefaultLanguage = false;

        /// <summary>
        ///     字符串
        /// </summary>
        public static StringResource MStringResource = new StringResource();

        /// <summary>
        ///     获得文字
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string GetText(TextType tag) => GetText(tag.ToString());

        /// <summary>
        ///     获得文字
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string GetText(string tag) => GetText(tag, tag);

        /// <summary>
        ///     获得文字
        /// </summary>
        /// <param name="defaultText"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string GetText(string defaultText, TextType tag) => GetText(defaultText, tag.ToString());

        /// <summary>
        ///     获得文字
        /// </summary>
        /// <param name="defaultText"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string GetText(string defaultText, string tag)
        {
            if (IsUseDefaultLanguage) return defaultText;
            tag = tag.Replace("_", string.Empty).ToUpper();
            string strText = string.Empty;
            StringResource.StringDic.TryGetValue(tag, out strText);
            strText = string.IsNullOrEmpty(strText) ? defaultText : strText.Replace("&amp;", "&");
            return strText;
        }

        /// <summary>
        ///     自动化多语言
        /// </summary>
        /// <param name="frm"></param>
        public static void Translateform(Form frm)
        {
            if (IsUseDefaultLanguage) return;
            if (frm.Tag != null)
            {
                var display = GetText(frm.Tag.ToString());
                if (!string.IsNullOrEmpty(display))
                {
                    frm.Text = display;
                }
            }
            //遍历所有控件
            Translateform(frm.Controls);
        }

        /// <summary>
        ///     控件多语言化
        /// </summary>
        /// <param name="controls"></param>
        public static void Translateform(ToolStripItemCollection controls)
        {
            foreach (ToolStripItem menuItem in controls)
            {
                if (menuItem.GetType().FullName == typeof(ToolStripSeparator).FullName) continue;
                if (menuItem.GetType().FullName == typeof(ToolStripMenuItem).FullName)
                {
                    if (menuItem.Tag == null) continue;
                    var display = GetText(menuItem.Tag.ToString());
                    if (string.IsNullOrEmpty(display)) continue;
                    menuItem.Text = display;
                    if (((ToolStripMenuItem)menuItem).DropDownItems.Count > 0)
                        Translateform(((ToolStripMenuItem)menuItem).DropDownItems);
                }
                if (menuItem.GetType().FullName == typeof(ToolStripButton).FullName)
                {
                    if (menuItem.Tag == null) continue;
                    var display = GetText(menuItem.Tag.ToString());
                    if (string.IsNullOrEmpty(display)) continue;
                    menuItem.Text = display;
                }
            }
        }

        /// <summary>
        ///     控件多语言化
        /// </summary>
        /// <param name="controls"></param>
        public static void Translateform(Control.ControlCollection controls)
        {
            var display = string.Empty;
            foreach (Control ctrlItem in controls)
            {
                //System.Diagnostics.Debug.WriteLine(ctrlItem.GetType().FullName);
                //复合控件
                if (ctrlItem.GetType().FullName == typeof(MenuStrip).FullName)
                {
                    if (((MenuStrip)ctrlItem).Items.Count > 0) Translateform(((MenuStrip)ctrlItem).Items);
                }
                //ToolStrip
                if (ctrlItem.GetType().FullName == typeof(ToolStrip).FullName)
                {
                    if (((ToolStrip)ctrlItem).Items.Count > 0) Translateform(((ToolStrip)ctrlItem).Items);
                }
                //Tab
                if (ctrlItem.GetType().FullName == typeof(TabControl).FullName)
                {
                    foreach (TabPage tab in ((TabControl)ctrlItem).TabPages)
                    {
                        if (tab.Tag != null)
                        {
                            display = GetText(tab.Tag.ToString());
                            tab.Text = display;
                        }
                        Translateform(tab.Controls);
                    }
                }
                //GroupBox
                if (ctrlItem.GetType().FullName == typeof(GroupBox).FullName)
                {
                    if (!string.IsNullOrEmpty(display))
                    {
                        ((GroupBox)ctrlItem).Text = display;
                    }
                    Translateform(ctrlItem.Controls);
                }
                //列表控件
                if (ctrlItem.GetType().FullName == typeof(ListView).FullName)
                {
                    ListView lst = (ListView)ctrlItem;
                    foreach (ColumnHeader header in lst.Columns)
                    {
                        if (header.Tag != null)
                        {
                            header.Text = GetText(header.Text, header.Tag.ToString());
                        }
                    }
                }
                //单一控件
                if (ctrlItem.Tag == null) continue;
                //标签
                if (ctrlItem.GetType().FullName == typeof(Label).FullName)
                {
                    ((Label)ctrlItem).Text = GetText(((Label)ctrlItem).Text, ctrlItem.Tag.ToString());
                }
                //按钮
                if (ctrlItem.GetType().FullName == typeof(Button).FullName)
                {
                    ((Button)ctrlItem).Text = GetText(((Button)ctrlItem).Text, ctrlItem.Tag.ToString());
                    if (ctrlItem.Tag.ToString() == TextType.CommonOK.ToString())
                    {
                        ((Button)ctrlItem).BackColor = SuccessColor;
                    }
                    if (ctrlItem.Tag.ToString() == TextType.CommonNo.ToString() ||
                        ctrlItem.Tag.ToString() == TextType.CommonClose.ToString() ||
                        ctrlItem.Tag.ToString() == TextType.CommonCancel.ToString())
                    {
                        ((Button)ctrlItem).BackColor = FailColor;
                    }
                }
                //复选框
                if (ctrlItem.GetType().FullName == typeof(CheckBox).FullName)
                {
                    ((CheckBox)ctrlItem).Text = GetText(((CheckBox)ctrlItem).Text, ctrlItem.Tag.ToString());
                }
                //单选框
                if (ctrlItem.GetType().FullName == typeof(RadioButton).FullName)
                {
                    ((RadioButton)ctrlItem).Text = GetText(((RadioButton)ctrlItem).Text, ctrlItem.Tag.ToString());
                }
            }
        }
    }
}