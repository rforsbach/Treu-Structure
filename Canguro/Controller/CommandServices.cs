using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Canguro.Controller
{
    public class CommandServices : IDisposable
    {
        private Controller controller;
        //private System.Threading.Thread thread;
        //private CommandPanelParams panel;
        private WaitingFor selectionFilter;
        private object waitingObj;
        private List<Model.Item> selectedItems = null;

        private static Microsoft.DirectX.Vector3 lastPoint = Microsoft.DirectX.Vector3.Empty;

        //public event EventHandler ShowCommandPanel;

        public CommandServices(Controller controller)
        {
            selectionFilter = WaitingFor.None;
            waitingObj = null;
            this.controller = controller;
            
            // Aquí se recibe el evento de cambio en la selección desde el Model
            //Canguro.Model.Model.Instance.SelectionChanged += new Canguro.Model.Model.SelectionChangedEventHandler(onSelectionChanged);
            
            // Este será para cuando el SmallPanel esté integrado con DirectX
            //Canguro.View.GraphicViewManager.Instance.EnterData += new Canguro.View.EnterDataEventHandler(onEnterData);
            
            // Este funciona para recibir el input del SmallPanel independiente
            controller.MainFrm.SmallPanel.EnterData += new Canguro.View.EnterDataEventHandler(onEnterData);
        }

        #region Selection handling
        public bool StoreSelection()
        {
            if (selectedItems == null)
                selectedItems = new List<Canguro.Model.Item>();
            else
                selectedItems.Clear();

            return getSelection(Model, selectedItems);
        }

        public void RestoreSelection()
        {
            if (selectedItems != null && selectedItems.Count > 0)
            {
                foreach (Model.Item item in selectedItems)
                    if (item != null)
                        item.IsSelected = true;
            }
        }

        /// <summary>
        /// If the Model has selected Items, they are put in one list and returned. Otherwise, the user is asked to select items.
        /// Returns a List of selected Items with at least one element.
        /// </summary>
        /// <param name="joints">Returns the list of selected joints</param>
        /// <param name="lines">Returns the list of selected lines</param>
        /// <returns>A boolean. True if selected items were found. False otherwise.</returns>
        public bool GetSelection(Dictionary<uint, Model.Joint> joints, List<Model.LineElement> lines, List<Model.AreaElement> areas)
        {
            List<Model.Item> selection = GetSelection();

            foreach (Model.Item item in selection)
            {
                if (item is Model.Joint)
                    joints.Add(item.Id, (Model.Joint)item);
                else if (item is Model.LineElement)
                {
                    Model.LineElement l = (Model.LineElement)item;
                    lines.Add(l);
                    if (!joints.ContainsKey(l.I.Id))
                        joints.Add(l.I.Id, l.I);
                    if (!joints.ContainsKey(l.J.Id))
                        joints.Add(l.J.Id, l.J);
                }
                else if (item is Model.AreaElement)
                {
                    Model.AreaElement a = (Model.AreaElement)item;
                    areas.Add(a);
                    if (!joints.ContainsKey(a.J1.Id))
                        joints.Add(a.J1.Id, a.J1);
                    if (!joints.ContainsKey(a.J2.Id))
                        joints.Add(a.J2.Id, a.J2);
                    if (!joints.ContainsKey(a.J3.Id))
                        joints.Add(a.J3.Id, a.J3);
                    if (a.J4 != null && !joints.ContainsKey(a.J4.Id))
                        joints.Add(a.J4.Id, a.J4);
                }
            }

            return joints.Count > 0 || lines.Count > 0 || areas.Count > 0;
        }

        /// <summary>
        /// If the Model has selected Items, they are put in one list and returned. Otherwise, the user is asked to select items.
        /// Returns a List of selected Items with at least one element.
        /// </summary>
        /// <returns>A list of selected Items with at least one element</returns>
        public List<Canguro.Model.Item> GetSelection()
        {
            List<Canguro.Model.Item> selection = new List<Canguro.Model.Item>();

            if (!getSelection(Model, selection))
            {
                GetMany();
                getSelection(Model, selection);
            }

            return selection;
        }

        /// <summary>
        /// Adds all the selected Items to the provided selection list.
        /// </summary>
        /// <param name="model">The current Model object</param>
        /// <param name="selection">The list to add all the selected Items.</param>
        /// <returns>true if selection contains Items. false otherwise</returns>
        private bool getSelection(Canguro.Model.Model model, List<Canguro.Model.Item> selection)
        {
            foreach (Canguro.Model.Item item in model.JointList)
                if (item != null && item.IsSelected)
                    selection.Add(item);

            foreach (Canguro.Model.Item item in model.LineList)
                if (item != null && item.IsSelected)
                    selection.Add(item);

            foreach (Canguro.Model.Item item in model.AreaList)
                if (item != null && item.IsSelected)
                    selection.Add(item);

            return (selection.Count > 0);
        }
        #endregion

        /// <summary>
        /// Method that responds to the EnterData event of SmallPanel.
        /// The text input by the user on the SmallPanel's TextBox (data) is
        /// parsed here. If the input is successfull locks are released and
        /// execution continues normally, i.e.: waitingObj is assigned to the 
        /// refered object and the selectionFilter is set to None. Otherwise, 
        /// the input is rejected.
        /// </summary>
        /// <param name="sender">Usually, the SmallPanel in MainFrm</param>
        /// <param name="e">Event args with the user input</param>
        void onEnterData(object sender, Canguro.View.EnterDataEventArgs e)
        {
            char[] charSeparators = new char[] {',', ';', ' ', '\t', '@'};
            waitingObj = null;

            switch (selectionFilter)
            {
                case WaitingFor.Point:
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        bool relativePt = false;
                        string trimmedPt = e.Data.Trim();                        
                        if (trimmedPt.Length > 0 && trimmedPt[0] == '@')
                            relativePt = true;

                        string[] pt = trimmedPt.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                        Microsoft.DirectX.Vector3 v;

                        try
                        {
                            if (pt.Length != 3) throw new FormatException();
                            v = new Microsoft.DirectX.Vector3(
                                float.Parse(pt[0]), float.Parse(pt[1]), float.Parse(pt[2]));
                            if (relativePt)
                                v += lastPoint;
                        }
                        catch (FormatException)
                        {
                            System.Windows.Forms.MessageBox.Show(
                                Culture.Get("enterDataError") + "'" + Culture.Get("enterDataPoint") + "'",
                                Culture.Get("enterDataErrorTitle"), System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Error);

                            return;
                        }                    
                        waitingObj = new Snap.PointMagnet(v);
                    }
                    
                    selectionFilter = WaitingFor.None;
                    break;

                case WaitingFor.Joint:
                    parseItem(e.Data, Canguro.Model.Model.Instance.JointList, Culture.Get("enterDataJoint"));
                    break;

                case WaitingFor.Line:
                    parseItem(e.Data, Canguro.Model.Model.Instance.LineList, Culture.Get("enterDataLine"));
                    break;

                case WaitingFor.Area:
                    parseItem(e.Data, Canguro.Model.Model.Instance.AreaList, Culture.Get("enterDataArea"));
                    break;

                case WaitingFor.Text:
                    waitingObj = e.Data;
                    selectionFilter = WaitingFor.None;
                    break;

                case WaitingFor.SimpleValue:
                    try
                    {
                        waitingObj = float.Parse(e.Data);
                        selectionFilter = WaitingFor.None;
                    }
                    catch (FormatException)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            Culture.Get("enterDataError") + "'" + Culture.Get("enterDataSimpleValue") + "'",
                            Culture.Get("enterDataErrorTitle"), System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Error);

                        return;
                    }
                    break;                    

                case WaitingFor.Any:
                case WaitingFor.Many:
                case WaitingFor.None:
                    break;

                default:
                    throw new NotImplementedException("CommandServices.onEnterData");
            }
        }

        /// <summary>
        /// Method that tries to assign waitingObj by parsing a string containing an Item 
        /// index and a list containing the actual objects being refered to. If successful, 
        /// it assigns waitingObj, selects the Item and sets the selectionFilter to None.
        /// If the string cannot be parsed or the index does not exist in the given list, 
        /// it displays an error to the user and returns without changing anything.
        /// </summary>
        /// <param name="parseStr">The user input</param>
        /// <param name="list">The list of Items where the search will be carried out (i.e. JointList)</param>
        /// <param name="dataStr">String with the Item being searched for in the proper language</param>
        private void parseItem(string parseStr, System.Collections.IList list, string dataStr)
        {
            if ((list != null) && (!string.IsNullOrEmpty(parseStr)))
            {
                try
                {
                    int index = int.Parse(parseStr.Trim());

                    if (index >= 0 && index < list.Count)
                        waitingObj = (Canguro.Model.Item)list[index];
                    else
                        throw new Canguro.Model.InvalidIndexException(dataStr);

                    if (waitingObj == null)
                        throw new Canguro.Model.InvalidIndexException(dataStr);
                }
                catch (FormatException)
                {
                    System.Windows.Forms.MessageBox.Show(
                        Culture.Get("enterDataError") + "'" + dataStr + "'",
                        Culture.Get("enterDataErrorTitle"), System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
                catch (Canguro.Model.InvalidIndexException)
                {
                    System.Windows.Forms.MessageBox.Show(
                        Culture.Get("enterDataIndexError").Replace("*type*", dataStr) + "'" + parseStr + "'",
                        Culture.Get("enterDataErrorTitle"), System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
            }

            if (waitingObj != null)
                ((Canguro.Model.Item)waitingObj).IsSelected = true;
            
            selectionFilter = WaitingFor.None;
        }

        /// <summary>
        /// Method that responds to a click on an Item. It releases any waiting locks, 
        /// i.e.sets waitingObj to pickedObj and the selectionFilter to None.
        /// </summary>
        /// <param name="pickedObj">The clicked Item</param>
        public void SelectionDone(object pickedObj)
        {
            switch (selectionFilter)
            {
                case WaitingFor.None:
                    waitingObj = null;
                    break;

                default:
                    waitingObj = pickedObj;
                    selectionFilter = WaitingFor.None;
                    break;
            }
        }

        /*
        void onSelectionChanged(object sender, Canguro.Model.Model.SelectionChangedEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        */

        /*
        public Commands.ModelCommand CurrentCommand
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
        */

        /// <summary>
        /// Facade to get the TrackingService from GraphicViewManager
        /// </summary>
        public Tracking.TrackingService TrackingService
        {
            get
            {
                return controller.TrackingController.TrackingService;
            }
            set
            {
                controller.TrackingController.TrackingService = value;
            }
        }


        public Snap.PointMagnet SnapPrimaryPoint
        {
            get { return controller.TrackingController.SnapController.PrimaryPoint; }
            private set { controller.TrackingController.SnapController.PrimaryPoint = value; }
        }

        /// <summary>
        /// Executes a command from another command's thread
        /// </summary>
        /// <param name="command"></param>
        public void Run(Commands.ModelCommand command)
        {
            command.Run(this);
        }

        /*
        public void Cancel()
        {
            throw new System.NotImplementedException();
        }
        */

        /// <summary>
        /// Method to allow commands to display their progress. If a command takes some 
        /// time to complete without user intervention, it should call this method. 
        /// </summary>
        /// <param name="mainProgress">Percentage completed of the whole command's execution</param>
        public void ReportProgress(uint mainProgress)
        {
            controller.MainFrm.ReportProgress(controller.ModelCommand.Title, mainProgress, "", 0, DateTime.Now);
        }

        /// <summary>
        /// Method to allow commands to display their progress. If a command takes some 
        /// time to complete without user intervention, it should call this method. 
        /// </summary>
        /// <param name="mainProgress">Percentage completed of the whole command's execution</param>
        /// <param name="subtaskText">Description of the command's current task</param>
        /// <param name="subtaskProgress">Percentage completed of the current command task being performed</param>
        public void ReportProgress(uint mainProgress, string subtaskText, uint subtaskProgress)
        {
            controller.MainFrm.ReportProgress(Culture.Get(controller.ModelCommand.Title), mainProgress, subtaskText, subtaskProgress, DateTime.Now);
        }

        /// <summary>
        /// Method to allow commands to print messages on the DirectX panel to inform the user in a
        /// not-so-invasive way.
        /// </summary>
        /// <param name="message">The text message to display</param>
        public void Print(string message)
        {
            // TODO: Esto se debe de cambiar para desplegar el texto en el Panel DirectX
            controller.MainFrm.SetStatusLabel(message);
        }

        /// <summary>
        /// Method to get a Joint from the user by any means available
        /// (i.e. picking, text input, etc.)
        /// </summary>
        /// <returns>The Joint selected by the user</returns>
        public Canguro.Model.Joint GetJoint()
        {
            return GetJoint(Culture.Get("getJoint"));
        }

        /// <summary>
        /// Method to get a Joint from the user by any means available
        /// (i.e. picking, text input, etc.)
        /// </summary>
        /// <param name="prompt">Text displayed on the SmallPanel, as instructions to the user</param>
        /// <returns>The Joint selected by the user</returns>
        public Canguro.Model.Joint GetJoint(string prompt)
        {
            controller.MainFrm.SmallPanel.Start(controller.ModelCommand.Title, prompt, 5, " 1 ");
            wait(WaitingFor.Joint, true);

            return (Canguro.Model.Joint)waitingObj;
        }

        /// <summary>
        /// This method requests a point and returns a joint. If a joint is found, the method gets it. If not, then creates one and if the created joint lies on a line,
        /// then the command splits the line and adds the new line to the newLines list. 
        /// </summary>
        /// <param name="newLines"> List of new lines created by this method while creating new joints </param>
        /// <returns> The created or found joint </returns>
        public Model.Joint GetJoint(IList<Model.LineElement> newLines)
        {
            Model.Joint joint;
            Canguro.Controller.Snap.Magnet magnet = GetPoint();
            if (magnet == null)
                return null;
            if (magnet is Canguro.Controller.Snap.PointMagnet && ((Canguro.Controller.Snap.PointMagnet)magnet).Joint != null)
                joint = ((Canguro.Controller.Snap.PointMagnet)magnet).Joint;
            else
            {
                Microsoft.DirectX.Vector3 v = magnet.SnapPosition;
                joint = new Canguro.Model.Joint(v.X, v.Y, v.Z);
                Model.JointList.Add(joint);

                if (magnet is Canguro.Controller.Snap.PointMagnet)
                {
                    Canguro.Controller.Snap.PointMagnet pmag = (Canguro.Controller.Snap.PointMagnet)magnet;
                    if (pmag.RelatedMagnets != null && pmag.RelatedMagnets.Count > 0)
                        for (int i = 0; i < pmag.RelatedMagnets.Count; i++)
                            if (pmag.RelatedMagnets[i] is Canguro.Controller.Snap.LineMagnet && ((Canguro.Controller.Snap.LineMagnet)pmag.RelatedMagnets[i]).Line != null)
                                Canguro.Commands.Model.SplitCmd.Split(((Canguro.Controller.Snap.LineMagnet)pmag.RelatedMagnets[i]).Line, joint, Model);
                }
                else if (magnet is Canguro.Controller.Snap.LineMagnet)
                {
                    Canguro.Controller.Snap.LineMagnet lmag = (Canguro.Controller.Snap.LineMagnet)magnet;
                    if (lmag.Line != null)
                        Canguro.Commands.Model.SplitCmd.Split(((Canguro.Controller.Snap.LineMagnet)magnet).Line, joint, Model);
                }
            }
            SnapPrimaryPoint = new Canguro.Controller.Snap.PointMagnet(joint);
            return joint;
        }

        /// <summary>
        /// Method to get a Line element from the user by any means available
        /// (i.e. picking, text input, etc.)
        /// </summary>
        /// <returns>The Line element selected by the user</returns>
        public Canguro.Model.LineElement GetLine()
        {
            return GetLine(Culture.Get("getLine"));
        }

        /// <summary>
        /// Method to get a Line element from the user by any means available
        /// (i.e. picking, text input, etc.)
        /// </summary>
        /// <param name="prompt">Text displayed on the SmallPanel, as instructions to the user</param>
        /// <returns>The Line element selected by the user</returns>
        public Canguro.Model.LineElement GetLine(string prompt)
        {
            controller.MainFrm.SmallPanel.Start(controller.ModelCommand.Title, prompt, 5, " 1 ");
            wait(WaitingFor.Line, true);

            return (Canguro.Model.LineElement)waitingObj;
        }

        /// <summary>
        /// Method to get any Item from the user by any means available
        /// (i.e. picking, text input, etc.)
        /// </summary>
        /// <returns>The Item selected by the user</returns>
        public Canguro.Model.Item GetItem()
        {
            return GetItem(Culture.Get("getItem"));
        }

        /// <summary>
        /// Method to get any Item from the user by any means available
        /// (i.e. picking, text input, etc.)
        /// </summary>
        /// <param name="prompt">Text displayed on the SmallPanel, as instructions to the user</param>
        /// <returns>The Item selected by the user</returns>
        public Canguro.Model.Item GetItem(string prompt)
        {
            controller.MainFrm.SmallPanel.Start(controller.ModelCommand.Title, prompt, 0, "");
            wait(WaitingFor.Any, true);

            return (Canguro.Model.Item)waitingObj;
        }

        /// <summary>
        /// Method to get many Items from the user by any means available
        /// (i.e. picking, text input, etc.)
        /// </summary>
        public void GetMany()
        {
            GetMany(Culture.Get("getMany"));
        }

        /// <summary>
        /// Method to get many Items from the user by any means available
        /// (i.e. picking, text input, etc.)
        /// </summary>
        /// <param name="prompt">Text displayed on the SmallPanel, as instructions to the user</param>
        public void GetMany(string prompt)
        {
            controller.MainFrm.SmallPanel.Start(controller.ModelCommand.Title, prompt, 0, "");
            wait(WaitingFor.Many, true);            
        }

        /// <summary>
        /// This method returns a Magnet whose SnapPoint property has the point obtained either typed or clicked.
        /// </summary>
        /// <returns>The Magnet whose SnapPoint was found.</returns>
        public Snap.Magnet GetPoint()
        {
            return GetPoint(Culture.Get("getPoint"));
        }

        /// <summary>
        /// This method returns a Magnet whose SnapPoint property has the point obtained either typed or clicked.
        /// </summary>
        /// <param name="prompt">The instruction text that appears at the SmallPanel.</param>
        /// <returns>The Magnet whose SnapPoint was found.</returns>
        public Snap.Magnet GetPoint(string prompt)
        {
            controller.MainFrm.SmallPanel.Start(controller.ModelCommand.Title, prompt, 10, "1,2,3 " + Culture.Get("or") +  " @1,2,3");
            wait(WaitingFor.Point, true);

            Snap.Magnet m = waitingObj as Snap.Magnet;
            if (m == null)
                return null;

            lastPoint = m.SnapPosition; 
            SnapPrimaryPoint = new Snap.PointMagnet(m.SnapPositionInt, Snap.PointMagnetType.SimplePoint);

            return m;
        }

        /// <summary>
        /// This method returns a vector (displacement) obtained by user input,
        /// either from clicking or typing. The input works by asking for a 
        /// basepoint and then for a second point, which can be typed with absolute
        /// or relative coordinates. The difference between the 2 points is returned.
        /// </summary>
        /// <param name="v">The vector (diplacement) to be returned or Vector3.Empty if
        /// no vector could be obtained.</param>
        /// <returns>True if a valid vector (displacement) could be obtained, false otherwise.</returns>
        public bool GetVector(out Microsoft.DirectX.Vector3 v)
        {
            Snap.Magnet m1, m2;

            // Ask for 1st point
            if ((m1 = GetPoint(Culture.Get("getVector1"))) == null)
            {
                v = Microsoft.DirectX.Vector3.Empty;
                return false;
            }
            
            // Activate vector Tracking
            Tracking.TrackingService ts = TrackingService;
            TrackingService = Tracking.VectorTrackingService.Instance;
            Microsoft.DirectX.Vector3 origin = m1.SnapPositionInt;
            TrackingService.SetPoint(origin);
            SnapPrimaryPoint = new Canguro.Controller.Snap.PointMagnet(origin, Canguro.Controller.Snap.PointMagnetType.SimplePoint);

            // Ask for 2nd point
            if ((m2 = GetPoint(Culture.Get("getVector2"))) == null)
            {
                v = Microsoft.DirectX.Vector3.Empty;
                TrackingService = ts;
                return false;
            }
            SnapPrimaryPoint = new Canguro.Controller.Snap.PointMagnet(m2.SnapPositionInt, Canguro.Controller.Snap.PointMagnetType.SimplePoint);

            // Calculate and return difference
            v = m2.SnapPositionInt - origin;
            Model.UnitSystem.UnitSystem us = Canguro.Model.UnitSystem.UnitSystemsManager.Instance.CurrentSystem;
            v.X = us.FromInternational(v.X, Canguro.Model.UnitSystem.Units.Distance);
            v.Y = us.FromInternational(v.Y, Canguro.Model.UnitSystem.Units.Distance);
            v.Z = us.FromInternational(v.Z, Canguro.Model.UnitSystem.Units.Distance);

            TrackingService = ts;
            return true;
        }

        /// <summary>
        /// Method to get a text string from the user
        /// </summary>
        /// <param name="prompt">Text displayed on the SmallPanel, as instructions to the user</param>
        /// <returns>The text string typed by the user</returns>
        public string GetString(string prompt)
        {
            controller.MainFrm.SmallPanel.Start(controller.ModelCommand.Title, prompt, 20, "");
            wait(WaitingFor.Text, true);

            return (string)waitingObj;
        }

        /// <summary>
        /// Method to get a float value from the user
        /// </summary>
        /// <param name="prompt">Text displayed on the SmallPanel, as instructions to the user</param>
        /// <returns>The float value typed by the user</returns>
        public float GetSingle(string prompt)
        {
            controller.MainFrm.SmallPanel.Start(controller.ModelCommand.Title, prompt, 20, "");
            wait(WaitingFor.Text, true);

            try
            {
                return float.Parse((string)waitingObj);
            }
            catch (FormatException) { return GetSingle(prompt); }
        }

        /*
        public void ShowPanel(CommandPanelParams parameters)
        {
            throw new System.NotImplementedException();
        }
        */

        /// <summary>
        /// Method to present the user with an option to view and change an object
        /// properties. It shows a PropertyGrid with which the user is allowed to 
        /// interact. Properties are changed in the object immediately after the user
        /// changes them in the PropertyGrid.
        /// </summary>
        /// <param name="obj">The object whose properties are being accessed</param>
        public void GetProperties(object obj)
        {
            GetProperties(controller.ModelCommand.Title, obj);
        }

        /// <summary>
        /// Method to present the user with an option to view and change an object
        /// properties. It shows a PropertyGrid with which the user is allowed to 
        /// interact. Properties are changed in the object immediately after the user
        /// changes them in the PropertyGrid.
        /// </summary>
        /// <param name="title">The title being displayed on the Titlebar of the PropertyGrid's window</param>
        /// <param name="obj">The object whose properties are being accessed</param>
        public void GetProperties(string title, object obj)
        {
            GetProperties(title, obj, true);
        }

        public System.Windows.Forms.DialogResult ShowDialog(System.Windows.Forms.Form form)
        {
            //form.Owner = controller.MainFrm;
            return form.ShowDialog(controller.MainFrm);
        }

        /// <summary>
        /// Method to present the user with an option to view and change an object
        /// properties. It shows a PropertyGrid with which the user is allowed to 
        /// interact. Properties are changed in the object immediately after the user
        /// changes them in the PropertyGrid.
        /// </summary>
        /// <param name="title">The title being displayed on the Titlebar of the PropertyGrid's window</param>
        /// <param name="obj">The object whose properties are being accessed</param>
        /// <param name="runAsync">Flag indicating whether the window should block the command
        /// until the user dismisses it or if it should be allowed to float while the command
        /// continues execution (the same as a modal vs. a modeless window)</param>
        public void GetProperties(string title, object obj, bool runAsync)
        {
            object[] objs = { obj };
            GetProperties(title, objs, runAsync, null);
        }

        /// <summary>
        /// Method to present the user with an option to view and change an object
        /// properties. It shows a PropertyGrid with which the user is allowed to 
        /// interact. Properties are changed in the object immediately after the user
        /// changes them in the PropertyGrid.
        /// </summary>
        /// <param name="title">The title being displayed on the Titlebar of the PropertyGrid's window</param>
        /// <param name="obj">The object whose properties are being accessed</param>
        /// <param name="runAsync">Flag indicating whether the window should block the command
        /// until the user dismisses it or if it should be allowed to float while the command
        /// continues execution (the same as a modal vs. a modeless window)</param>
        /// <param name="listItems">Options to be shown at the CommandBox in the 
        /// CommandToolbox. If null, no CommandBox is shown.</param>
        public void GetProperties(string title, object[] obj, bool runAsync, string[] listItems)
        {
            controller.MainFrm.ShowCommandToolbox(title, obj, runAsync, listItems);
        }

        /// <summary>
        /// Gets the only and global Model instance
        /// </summary>
        public Canguro.Model.Model Model
        {
            get
            {
                return Canguro.Model.Model.Instance;
            }
        }

        /// <summary>
        /// Method that locks commands when waiting for user interaction, until the requested
        /// action has been performed. For example, if a command requests for a Joint, then it
        /// gets locked until the user picks one by any means available.
        /// </summary>
        /// <param name="waitingFor">The object that was picked by the user</param>
        /// <param name="startSelection">The type of object that the command requests (i.e. Joint, Line, Point, etc.)</param>
        private void wait(WaitingFor waitingFor, bool startSelection)
        {
            Commands.ModelCommand mc;
            selectionFilter = waitingFor;

            if (startSelection)
                controller.SelectionCommand.Start(this);

            while (selectionFilter != WaitingFor.None)
            {
                // if ModelCmd has been cancelled or ModelCmd is no longer registered on the Controller
                // Cancel the command and stop the waiting
                if (((mc = controller.ModelCommand) == null) || ((mc != null) && (mc.Cancel)))
                    throw new CancelCommandException();

                // Para evitar que este ciclo se coma todo el cpu, porque
                // DoEvents sólo pasa el control al MessageLoop de la aplicación
                // para que cheque si hay mensajes y en caso contrario regresa
                // inmediatamente, lo que hace que este ciclo se convierta en un
                // while que consume el 100% del cpu nomás por esperar
                // (Exactamente como el PropertyGrid cuando despliega un DropDown)
                // La llamada a la rutina de abajo duerme al hilo hasta que ocurra
                // cualquier evento, con lo que se arregla el problema
                Canguro.Utility.NativeHelperMethods.WaitInMainThread(250);
            }
        }

        /*
        public class ShowCommandPanelEventArgs : EventArgs
        {
            public CommandPanelParams parameters;
        }
        */

        /// <summary>
        /// Gets the SelectionFilter, which filters the input from the user to acommodate the
        /// command's request. For example, if the command asks for a Joint and the user picks
        /// a Line, it gets filtered until the user picks a Joint.
        /// </summary>
        public WaitingFor SelectionFilter
        {
            get
            {
                return selectionFilter;
            }
        }

        internal Credentials UserCredentials
        {
            get { return controller.UserCredentials; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            // Este funciona para dejar de recibir el input del SmallPanel independiente
            controller.MainFrm.SmallPanel.EnterData -= new Canguro.View.EnterDataEventHandler(onEnterData);
        }

        #endregion
    }
}
