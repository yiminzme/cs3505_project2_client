﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by coded UI test builder.
//      Version: 14.0.0.0
//
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------

namespace CodedUITestProject1
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    
    
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public partial class UIMap
    {
        
        /// <summary>
        /// Test to ensure that you can enter strings in the form
        /// </summary>
        public void CanTypeStrings()
        {
            #region Variable Declarations
            WinEdit uINameEdit = this.UIDebugWindow.UIItemWindow.UIWindowsFormsApplicatListItem.UINameEdit;
            WinWindow uISuperSpreadsheetWindow = this.UISuperSpreadsheetWindow;
            WinClient uISpreadsheetPanel1Client = this.UISuperSpreadsheetWindow.UIItemWindow.UISpreadsheetPanel1Client;
            WinEdit uICellContentsBoxEdit = this.UISuperSpreadsheetWindow.UICellContentsBoxWindow.UICellContentsBoxEdit;
            #endregion

            // Double-Click 'Name' text box
            Mouse.DoubleClick(uINameEdit, new Point(21, 6));

            // Maximize window 'Super Spreadsheet'
            uISuperSpreadsheetWindow.Maximized = this.CanTypeStringsParams.UISuperSpreadsheetWindowMaximized;

            // Click 'spreadsheetPanel1' client
            Mouse.Click(uISpreadsheetPanel1Client, new Point(77, 47));

            // Type 'This is a Test to ensure Strings Work' in 'cellContentsBox' text box
            uICellContentsBoxEdit.Text = this.CanTypeStringsParams.UICellContentsBoxEditText;

            // Type '{Enter}' in 'cellContentsBox' text box
            Keyboard.SendKeys(uICellContentsBoxEdit, this.CanTypeStringsParams.UICellContentsBoxEditSendKeys, ModifierKeys.None);
        }
        
        /// <summary>
        /// Asserts that we can enter strings.
        /// </summary>
        public void AssertThatCanEnterString()
        {
            #region Variable Declarations
            WinEdit uICellContentsBoxEdit = this.UISuperSpreadsheetWindow.UICellContentsBoxWindow.UICellContentsBoxEdit;
            WinEdit uICellValueBoxEdit = this.UISuperSpreadsheetWindow.UICellValueBoxWindow.UICellValueBoxEdit;
            WinEdit uICellNameBoxEdit = this.UISuperSpreadsheetWindow.UIA1Window.UICellNameBoxEdit;
            #endregion

            // Verify that the 'Text' property of 'cellContentsBox' text box equals 'This is a Test to ensure Strings Work'
            Assert.AreEqual(this.AssertThatCanEnterStringExpectedValues.UICellContentsBoxEditText, uICellContentsBoxEdit.Text, "Your text wasn\'t saved");

            // Verify that the 'Text' property of 'cellValueBox' text box equals 'This is a Test to ensure Strings Work'
            Assert.AreEqual(this.AssertThatCanEnterStringExpectedValues.UICellValueBoxEditText, uICellValueBoxEdit.Text, "Your Value box failed.");

            // Verify that the 'Text' property of 'cellNameBox' text box equals 'A1'
            Assert.AreEqual(this.AssertThatCanEnterStringExpectedValues.UICellNameBoxEditText, uICellNameBoxEdit.Text, "Your Location Box Failed");
        }
        
        #region Properties
        public virtual CanTypeStringsParams CanTypeStringsParams
        {
            get
            {
                if ((this.mCanTypeStringsParams == null))
                {
                    this.mCanTypeStringsParams = new CanTypeStringsParams();
                }
                return this.mCanTypeStringsParams;
            }
        }
        
        public virtual AssertThatCanEnterStringExpectedValues AssertThatCanEnterStringExpectedValues
        {
            get
            {
                if ((this.mAssertThatCanEnterStringExpectedValues == null))
                {
                    this.mAssertThatCanEnterStringExpectedValues = new AssertThatCanEnterStringExpectedValues();
                }
                return this.mAssertThatCanEnterStringExpectedValues;
            }
        }
        
        public UIDebugWindow UIDebugWindow
        {
            get
            {
                if ((this.mUIDebugWindow == null))
                {
                    this.mUIDebugWindow = new UIDebugWindow();
                }
                return this.mUIDebugWindow;
            }
        }
        
        public UISuperSpreadsheetWindow UISuperSpreadsheetWindow
        {
            get
            {
                if ((this.mUISuperSpreadsheetWindow == null))
                {
                    this.mUISuperSpreadsheetWindow = new UISuperSpreadsheetWindow();
                }
                return this.mUISuperSpreadsheetWindow;
            }
        }
        #endregion
        
        #region Fields
        private CanTypeStringsParams mCanTypeStringsParams;
        
        private AssertThatCanEnterStringExpectedValues mAssertThatCanEnterStringExpectedValues;
        
        private UIDebugWindow mUIDebugWindow;
        
        private UISuperSpreadsheetWindow mUISuperSpreadsheetWindow;
        #endregion
    }
    
    /// <summary>
    /// Parameters to be passed into 'CanTypeStrings'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class CanTypeStringsParams
    {
        
        #region Fields
        /// <summary>
        /// Maximize window 'Super Spreadsheet'
        /// </summary>
        public bool UISuperSpreadsheetWindowMaximized = true;
        
        /// <summary>
        /// Type 'This is a Test to ensure Strings Work' in 'cellContentsBox' text box
        /// </summary>
        public string UICellContentsBoxEditText = "This is a Test to ensure Strings Work";
        
        /// <summary>
        /// Type '{Enter}' in 'cellContentsBox' text box
        /// </summary>
        public string UICellContentsBoxEditSendKeys = "{Enter}";
        #endregion
    }
    
    /// <summary>
    /// Parameters to be passed into 'AssertThatCanEnterString'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class AssertThatCanEnterStringExpectedValues
    {
        
        #region Fields
        /// <summary>
        /// Verify that the 'Text' property of 'cellContentsBox' text box equals 'This is a Test to ensure Strings Work'
        /// </summary>
        public string UICellContentsBoxEditText = "This is a Test to ensure Strings Work";
        
        /// <summary>
        /// Verify that the 'Text' property of 'cellValueBox' text box equals 'This is a Test to ensure Strings Work'
        /// </summary>
        public string UICellValueBoxEditText = "This is a Test to ensure Strings Work";
        
        /// <summary>
        /// Verify that the 'Text' property of 'cellNameBox' text box equals 'A1'
        /// </summary>
        public string UICellNameBoxEditText = "A1";
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class UIDebugWindow : WinWindow
    {
        
        public UIDebugWindow()
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.Name] = "Debug";
            this.SearchProperties[WinWindow.PropertyNames.ClassName] = "CabinetWClass";
            this.WindowTitles.Add("Debug");
            #endregion
        }
        
        #region Properties
        public UIItemWindow UIItemWindow
        {
            get
            {
                if ((this.mUIItemWindow == null))
                {
                    this.mUIItemWindow = new UIItemWindow(this);
                }
                return this.mUIItemWindow;
            }
        }
        #endregion
        
        #region Fields
        private UIItemWindow mUIItemWindow;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class UIItemWindow : WinWindow
    {
        
        public UIItemWindow(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.AccessibleName] = "Items View";
            this.SearchProperties[WinWindow.PropertyNames.ClassName] = "DirectUIHWND";
            this.WindowTitles.Add("Debug");
            #endregion
        }
        
        #region Properties
        public UIWindowsFormsApplicatListItem UIWindowsFormsApplicatListItem
        {
            get
            {
                if ((this.mUIWindowsFormsApplicatListItem == null))
                {
                    this.mUIWindowsFormsApplicatListItem = new UIWindowsFormsApplicatListItem(this);
                }
                return this.mUIWindowsFormsApplicatListItem;
            }
        }
        #endregion
        
        #region Fields
        private UIWindowsFormsApplicatListItem mUIWindowsFormsApplicatListItem;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class UIWindowsFormsApplicatListItem : WinListItem
    {
        
        public UIWindowsFormsApplicatListItem(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinListItem.PropertyNames.Name] = "WindowsFormsApplication1.exe";
            this.WindowTitles.Add("Debug");
            #endregion
        }
        
        #region Properties
        public WinEdit UINameEdit
        {
            get
            {
                if ((this.mUINameEdit == null))
                {
                    this.mUINameEdit = new WinEdit(this);
                    #region Search Criteria
                    this.mUINameEdit.SearchProperties[WinEdit.PropertyNames.Name] = "Name";
                    this.mUINameEdit.WindowTitles.Add("Debug");
                    #endregion
                }
                return this.mUINameEdit;
            }
        }
        #endregion
        
        #region Fields
        private WinEdit mUINameEdit;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class UISuperSpreadsheetWindow : WinWindow
    {
        
        public UISuperSpreadsheetWindow()
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.Name] = "Super Spreadsheet";
            this.SearchProperties.Add(new PropertyExpression(WinWindow.PropertyNames.ClassName, "WindowsForms10.Window", PropertyExpressionOperator.Contains));
            this.WindowTitles.Add("Super Spreadsheet");
            this.WindowTitles.Add("Super Spreadsheet*");
            #endregion
        }
        
        #region Properties
        public UIItemWindow1 UIItemWindow
        {
            get
            {
                if ((this.mUIItemWindow == null))
                {
                    this.mUIItemWindow = new UIItemWindow1(this);
                }
                return this.mUIItemWindow;
            }
        }
        
        public UICellContentsBoxWindow UICellContentsBoxWindow
        {
            get
            {
                if ((this.mUICellContentsBoxWindow == null))
                {
                    this.mUICellContentsBoxWindow = new UICellContentsBoxWindow(this);
                }
                return this.mUICellContentsBoxWindow;
            }
        }
        
        public UICellValueBoxWindow UICellValueBoxWindow
        {
            get
            {
                if ((this.mUICellValueBoxWindow == null))
                {
                    this.mUICellValueBoxWindow = new UICellValueBoxWindow(this);
                }
                return this.mUICellValueBoxWindow;
            }
        }
        
        public UIA1Window UIA1Window
        {
            get
            {
                if ((this.mUIA1Window == null))
                {
                    this.mUIA1Window = new UIA1Window(this);
                }
                return this.mUIA1Window;
            }
        }
        #endregion
        
        #region Fields
        private UIItemWindow1 mUIItemWindow;
        
        private UICellContentsBoxWindow mUICellContentsBoxWindow;
        
        private UICellValueBoxWindow mUICellValueBoxWindow;
        
        private UIA1Window mUIA1Window;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class UIItemWindow1 : WinWindow
    {
        
        public UIItemWindow1(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties.Add(new PropertyExpression(WinWindow.PropertyNames.ClassName, "WindowsForms10.Window", PropertyExpressionOperator.Contains));
            this.SearchProperties[WinWindow.PropertyNames.Instance] = "2";
            this.WindowTitles.Add("Super Spreadsheet");
            #endregion
        }
        
        #region Properties
        public WinClient UISpreadsheetPanel1Client
        {
            get
            {
                if ((this.mUISpreadsheetPanel1Client == null))
                {
                    this.mUISpreadsheetPanel1Client = new WinClient(this);
                    #region Search Criteria
                    this.mUISpreadsheetPanel1Client.WindowTitles.Add("Super Spreadsheet");
                    #endregion
                }
                return this.mUISpreadsheetPanel1Client;
            }
        }
        #endregion
        
        #region Fields
        private WinClient mUISpreadsheetPanel1Client;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class UICellContentsBoxWindow : WinWindow
    {
        
        public UICellContentsBoxWindow(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.ControlName] = "cellContentsBox";
            this.WindowTitles.Add("Super Spreadsheet");
            this.WindowTitles.Add("Super Spreadsheet*");
            #endregion
        }
        
        #region Properties
        public WinEdit UICellContentsBoxEdit
        {
            get
            {
                if ((this.mUICellContentsBoxEdit == null))
                {
                    this.mUICellContentsBoxEdit = new WinEdit(this);
                    #region Search Criteria
                    this.mUICellContentsBoxEdit.WindowTitles.Add("Super Spreadsheet");
                    this.mUICellContentsBoxEdit.WindowTitles.Add("Super Spreadsheet*");
                    #endregion
                }
                return this.mUICellContentsBoxEdit;
            }
        }
        #endregion
        
        #region Fields
        private WinEdit mUICellContentsBoxEdit;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class UICellValueBoxWindow : WinWindow
    {
        
        public UICellValueBoxWindow(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.ControlName] = "cellValueBox";
            this.WindowTitles.Add("Super Spreadsheet*");
            #endregion
        }
        
        #region Properties
        public WinEdit UICellValueBoxEdit
        {
            get
            {
                if ((this.mUICellValueBoxEdit == null))
                {
                    this.mUICellValueBoxEdit = new WinEdit(this);
                    #region Search Criteria
                    this.mUICellValueBoxEdit.WindowTitles.Add("Super Spreadsheet*");
                    #endregion
                }
                return this.mUICellValueBoxEdit;
            }
        }
        #endregion
        
        #region Fields
        private WinEdit mUICellValueBoxEdit;
        #endregion
    }
    
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class UIA1Window : WinWindow
    {
        
        public UIA1Window(UITestControl searchLimitContainer) : 
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.ControlName] = "cellNameBox";
            this.WindowTitles.Add("Super Spreadsheet*");
            #endregion
        }
        
        #region Properties
        public WinEdit UICellNameBoxEdit
        {
            get
            {
                if ((this.mUICellNameBoxEdit == null))
                {
                    this.mUICellNameBoxEdit = new WinEdit(this);
                    #region Search Criteria
                    this.mUICellNameBoxEdit.WindowTitles.Add("Super Spreadsheet*");
                    #endregion
                }
                return this.mUICellNameBoxEdit;
            }
        }
        #endregion
        
        #region Fields
        private WinEdit mUICellNameBoxEdit;
        #endregion
    }
}