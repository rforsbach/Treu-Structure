using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Canguro.Controller.Tracking;

namespace Canguro.Commands.View
{
    /// <summary>
    /// View Command to select objects, either with picking or window.
    /// </summary>
    public class Selection : Canguro.Commands.ViewCommand
    {
        #region Fields and Isinteractive
        public const int PickHalfWidthPixels = 5;
        public const int PickCycleMoveTolerance = 1;
        private const int minimumSelectionBufferSize = 1000;

        byte[] jointPos = new byte[minimumSelectionBufferSize];
        float[] jointPosY = new float[minimumSelectionBufferSize];
        float[] jointPosX = new float[minimumSelectionBufferSize];

        private int fx, fy;
        private int wfx, wfy;
        private int lastPickX = -1, lastPickY = -1;
        //private List<SelectedItem> candidatesPickList = null;
        private List<Canguro.Model.Item> candidatesPickList = null;
        private int lastCandidatePicked = -1;
        private Canguro.Controller.CommandServices services;
        private System.Windows.Forms.Cursor cursorDefault = new System.Windows.Forms.Cursor(typeof(MainFrm), "Commands.SelectDefault.cur");
        private System.Windows.Forms.Cursor cursorPick = new System.Windows.Forms.Cursor(typeof(MainFrm), "Commands.Pick.cur");
        private System.Windows.Forms.Cursor cursorSelect = System.Windows.Forms.Cursors.Cross;
        private System.Windows.Forms.Cursor cursor;
        private bool windowSelectionOn = false;
        private bool crossingSelectionOn = false;
        private bool newlySelected = false;

        public override bool IsInteractive
        {
            get
            {
                return true;
            }
        }

        public bool WindowSelectionOn
        {
            get { return windowSelectionOn || crossingSelectionOn; }
        }
        #endregion

        #region Constructors
        private Selection()
        {
            cursor = cursorDefault;
        }

        public static Selection Instance = new Selection();

        public void Start(Canguro.Controller.CommandServices services)
        {
            if (services == null)
            {
                this.services = null;
                Reset();
                return;
            }

            Canguro.Model.Model m = Canguro.Model.Model.Instance;

            // If it's a new command asking for its first selection, throw current (past) selection away
            if (this.services != services)
                m.UnSelectAll();

            this.services = services;
            Reset();


            // Start selection command
            Canguro.Controller.Controller.Instance.Execute("select");

            // If starting a Get Point Selection, start Snap
            if (services.SelectionFilter == Canguro.Controller.WaitingFor.Point)
                Controller.Controller.Instance.TrackingController.SnapController.IsActive = true;
        }
        #endregion

        #region Cursor
        void resetCursor()
        {
            if (services == null)
                setCursor(Canguro.Controller.WaitingFor.None);
            else
                setCursor(services.SelectionFilter);
        }
        
        void setCursor(Canguro.Controller.WaitingFor selectionFilter)
        {
            // Set cursor
            if (selectionFilter == Canguro.Controller.WaitingFor.Point)
                cursor = cursorSelect;
            else if ((selectionFilter == Canguro.Controller.WaitingFor.SimpleValue) ||
                    (selectionFilter == Canguro.Controller.WaitingFor.Text) ||
                    (selectionFilter == Canguro.Controller.WaitingFor.None))
                cursor = cursorDefault;
            else
                cursor = cursorPick;
        }

        public override System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return cursor;
            }
        }

        public System.Windows.Forms.Cursor DefaultCursor
        {
            get
            {
                return cursorDefault;
            }
        }
        #endregion

        #region Reset and endCycle
        /// <summary>
        /// End current selection cycle and puts Selection in a default state, except for
        /// the currently assigned CommandServices, which link Selection to the current
        /// Model Command under execution (if any). For a hard Reset which sets 
        /// CommandServices = null use Selection.Start(null).
        /// </summary>
        public void Reset()
        {
            endWindowSelection(false, null, Canguro.Controller.WaitingFor.None);
        }

        private void endCycle(object pickedObject)
        {
            if (services != null)
                services.SelectionDone(pickedObject);

            // Stop Snap
            Controller.Controller.Instance.TrackingController.SnapController.IsActive = false;

            Reset();
        }
        #endregion

        #region Window Selection
        void startWindowSelection(System.Drawing.Point location)
        {
            setCursor(Canguro.Controller.WaitingFor.Point);
            windowSelectionOn = true;
            wfx = fx;
            wfy = fy;

            startTracking(location);
        }

        void endWindowSelection(bool performSelection, Canguro.View.GraphicView activeView, Canguro.Controller.WaitingFor wf)
        {
            if (performSelection)
            {
                if (wfx > fx)
                {
                    int ffx = fx;
                    fx = wfx;
                    wfx = ffx;
                }
                if (wfy > fy)
                {
                    int ffy = fy;
                    fy = wfy;
                    wfy = ffy;
                }

                SelectWindow(activeView, wfx, wfy, fx, fy, null, Canguro.Controller.SelectionFilter.Joints | Canguro.Controller.SelectionFilter.Lines | Canguro.Controller.SelectionFilter.Areas);
                
                // TODO: Esto no está del todo bien porque avisa directo al Modelo
                if (newlySelected)
                    Canguro.Model.Model.Instance.ChangeSelection(null);
            }

            if (windowSelectionOn)
                endTracking();

            windowSelectionOn = false;
            newlySelected = false;
            crossingSelectionOn = false;

            resetCursor();
        }
        #endregion

        #region MouseEvents
        /// <summary>
        /// Responds to MouseDown event. Performs picking or selection (start and end of
        /// window selection) according to the current SelectionFilter. It also sends a 
        /// signal to CommandServices when selection is over so as to unlock waiting commands.
        /// </summary>
        /// <param name="activeView">The active View</param>
        /// <param name="e">MouseDown event arguments</param>
        public override void ButtonDown(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            fx = e.X;
            fy = e.Y;

            object pickedObj = null;
            Canguro.Model.Model m = Canguro.Model.Model.Instance;
            Canguro.Controller.WaitingFor wf;
    
            if (services == null)
                wf = Canguro.Controller.WaitingFor.Many;
            else
                wf = services.SelectionFilter;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                switch (wf)
                {
                    case Canguro.Controller.WaitingFor.None:
                    case Canguro.Controller.WaitingFor.SimpleValue:
                    case Canguro.Controller.WaitingFor.Text:
                        return;
                    case Canguro.Controller.WaitingFor.Point:
                        pickedObj = pickPoint(activeView);
                        break;
                    case Canguro.Controller.WaitingFor.Joint:
                    case Canguro.Controller.WaitingFor.Line:
                    case Canguro.Controller.WaitingFor.Area:
                    case Canguro.Controller.WaitingFor.Any:
                        pickedObj = pick(fx, fy, wf);
                        break;
                    case Canguro.Controller.WaitingFor.Many:
                        if (windowSelectionOn)
                        {
                            endWindowSelection(true, activeView, wf);
                            pickedObj = null;
                        }
                        else
                        {
                            // TODO: Try to get Anything by Picking and assign it to pickedObj
                            pickedObj = pick(fx, fy, wf);

                            // If nothing was picked turn to window selection mode
                            if (pickedObj == null)
                            {
                                startWindowSelection(e.Location);
                            }
                        }
                        break;

                    default:
                        throw new NotImplementedException("Selection.ButtonDown: " + wf.ToString());                
                }
                // If something was picked
                if (pickedObj != null)
                {
                    // If an Item was picked, select it
                    if (wf == Canguro.Controller.WaitingFor.Many)
                    {
                        if (pickedObj is Canguro.Model.Item)
                        {
                            ((Canguro.Model.Item)pickedObj).IsSelected = !((Canguro.Model.Item)pickedObj).IsSelected;
                            Canguro.Model.Model.Instance.ChangeSelection((Canguro.Model.Item)pickedObj);
                        }
                    }
                    else
                        //// Send signal (and item) to CommandServices if selection is over
                        //// (SelectionFilter != WaitingFor.Many)
                        //if (wf != Canguro.Controller.WaitingFor.Many)
                        endCycle(pickedObj);
                }
            }
        }

        /// <summary>
        /// If Right Mouse Button was clicked, then end current selection cycle
        /// </summary>
        /// <param name="activeView">The active View</param>
        /// <param name="e">MouseUp event arguments</param>
        public override void ButtonUp(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            // If right Button Click, end current selection cycle
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                endCycle(null);
        }

        public override void MouseMove(Canguro.View.GraphicView activeView, System.Windows.Forms.MouseEventArgs e)
        {
            if (windowSelectionOn)
            {
                // Switch between select Window and select Crossing 
                // depending on the direction of the bounding rectangle
                RectangleTrackingService rts;
                rts = Canguro.Controller.Controller.Instance.TrackingController.TrackingService as RectangleTrackingService;

                if (rts != null)
                {
                    if (!crossingSelectionOn && (e.Location.X - wfx) < 0)
                    {
                        rts.SetColor(System.Drawing.Color.FromArgb(120, 173, 255, 200), System.Drawing.Color.Green);
                        crossingSelectionOn = true;
                    }
                    else if (crossingSelectionOn && (e.Location.X - wfx) >= 0)
                    {
                        rts.SetColor();
                        crossingSelectionOn = false;
                    }
                }
            }
        }
        #endregion

        #region Tracking
        private void startTracking(System.Drawing.Point p)
        {
            TrackingService ts;
            ts = Canguro.Controller.Controller.Instance.TrackingController.TrackingService = 
                RectangleTrackingService.Instance;
            ts.SetPoint(p);
        }

        private void endTracking()
        {
            Canguro.Controller.Controller.Instance.TrackingController.TrackingService = null;
        }
        #endregion

        #region Picking and Selection
        private Controller.Snap.Magnet pickPoint(Canguro.View.GraphicView activeView)
        {
            return Controller.Controller.Instance.TrackingController.SnapController.SnapMagnet;
            //Controller.Snap.Magnet magnet = Controller.Controller.Instance.TrackingController.SnapController.SnapMagnet;
            //if (magnet != null)
            //    return magnet.SnapPosition;

            //Vector3 v = new Vector3(fx, fy, 0.5f);
            //activeView.Unproject(ref v);
            //return v;
        }

        private object pick(int x, int y, Controller.WaitingFor wf)
        {
            object firstItem = null, item = pick(x, y);
            if (item != null)
            {
                firstItem = item;
                do
                {
                    // Recorrer toda la candidatesPickList en busca de algún match
                    switch (wf)
                    {
                        case Canguro.Controller.WaitingFor.Any:
                        case Canguro.Controller.WaitingFor.Many:
                            return item;
                        case Canguro.Controller.WaitingFor.Area:
                            if (item is Canguro.Model.AreaElement) return item;
                            break;
                        case Canguro.Controller.WaitingFor.Joint:
                            if (item is Canguro.Model.Joint) return item;
                            break;
                        case Canguro.Controller.WaitingFor.Line:
                            if (item is Canguro.Model.LineElement) return item;
                            break;
                    }
                    item = pick(x, y);                  
                } while (item != firstItem);
            }

            return null;
        }

        private object pick(int x, int y)
        {
            // If mouse has moved, generate empty List to store candidates and throw last list away
            if ((Math.Abs(lastPickX - fx) > PickCycleMoveTolerance || 
                Math.Abs(lastPickY - fy) > PickCycleMoveTolerance) || 
                candidatesPickList == null)
            {
                candidatesPickList = Canguro.View.GraphicViewManager.Instance.PickItem(x, y);
                lastCandidatePicked = -1;
                lastPickX = fx;
                lastPickY = fy;
            }

            // Return next pick (if the list has recently been created, the first element will be returned)
            int unselectIndex = lastCandidatePicked;
            lastCandidatePicked++;
            if (lastCandidatePicked >= candidatesPickList.Count)
                lastCandidatePicked = 0;

            int count;
            if (candidatesPickList != null && ((count = candidatesPickList.Count) > 0))
            {
                // Unselect previously selected Item
                if (unselectIndex >= 0 && count > 1)
                {
                    candidatesPickList[unselectIndex].IsSelected = false;
                    Canguro.Model.Model.Instance.ChangeSelection(candidatesPickList[unselectIndex]);
                }
                return candidatesPickList[lastCandidatePicked];
            }
            else
                return null;
        }

        #region pickTmp
        /*
        private object pick(Canguro.View.GraphicView activeView, int x, int y, Controller.WaitingFor wf)
        {
            // Calculate ray and tolerance
            Vector3 rayP = new Vector3(x, y, 0.5f);
            Vector3 rayP2 = new Vector3(x, y, 1);
            Vector3 rayT1 = new Vector3(x + PickHalfWidthPixels, y, 0.5f);
            Vector3 rayT2 = new Vector3(x + PickHalfWidthPixels, y, 1);
            activeView.Unproject(ref rayP);
            activeView.Unproject(ref rayP2);
            activeView.Unproject(ref rayT1);
            activeView.Unproject(ref rayT2);

            Vector3 ray = rayP2 - rayP;
            ray.Normalize();

            Vector3 rayTolerance = (rayT2 - rayT1);
            rayTolerance.Normalize();

            float toleranceBiasSq = (rayT1 - rayP).Length();
            float toleranceSlopeSq = (rayT2 - rayP2).Length() - toleranceBiasSq;
            float tolerance2xBiasxSlope = 2.0f * toleranceBiasSq * toleranceSlopeSq;
            toleranceBiasSq *= toleranceBiasSq;
            toleranceSlopeSq *= toleranceSlopeSq;

            // If mouse has moved, generate empty List to store candidates and throw last list away
            if ((Math.Abs(lastPickX - fx) > 1 || Math.Abs(lastPickY - fy) > 1) || candidatesPickList == null)
            {
                candidatesPickList = new List<SelectedItem>(100);
                lastCandidatePicked = -1;
                lastPickX = fx;
                lastPickY = fy;

                // Calculate for Joints
                //        _   _        _ _  _              _         _    _
                // dSq = (r - p)^2 - ((r-p).u0)^2, point = r, line = p + tq
                double dSq, dDist;
                Vector3 vTmp;
                if ((wf == Canguro.Controller.WaitingFor.Joint || wf == Canguro.Controller.WaitingFor.Any || wf == Canguro.Controller.WaitingFor.Many)
                    && Canguro.Model.Model.Instance.JointList != null)
                {
                    foreach (Canguro.Model.Joint j in Canguro.Model.Model.Instance.JointList)
                    {
                        if (j != null)
                        {
                            // Calculate distance
                            vTmp = j.Position - rayP;
                            dDist = Vector3.Dot(vTmp, ray);
                            dSq = Vector3.Dot(vTmp, vTmp) - dDist * dDist;

                            // d^2 < (toleranceBias + dDist*toleranceSlope)^2
                            if (dSq < (toleranceBiasSq + dDist * dDist * toleranceSlopeSq + dDist * tolerance2xBiasxSlope))
                            {
                                // Add candidate for selection
                                candidatesPickList.Add(new SelectedItem(dDist, j));
                            }
                        }
                    }
                }

                // Calculate for Lines
                //         _   _    _          _   _    _      _   _    _
                // dist = (q - p) . n0,     g: x = p + ru   h: x = q + sv
                double dist;
                Vector3 u, n0;
                if ((wf == Canguro.Controller.WaitingFor.Line || wf == Canguro.Controller.WaitingFor.Any || wf == Canguro.Controller.WaitingFor.Many)
                    && Canguro.Model.Model.Instance.LineList != null)
                {
                    foreach (Canguro.Model.LineElement l in Canguro.Model.Model.Instance.LineList)
                    {
                        if (l != null)
                        {
                            // Obtain LineElement parametric form
                            u = l.J.Position - l.I.Position;
                            u.Normalize();

                            n0 = Vector3.Cross(u, ray);
                            n0.Normalize();

                            dist = Vector3.Dot((rayP - l.I.Position), n0);

                            // Falta encontrar altura a la que la dist fue calculada

                        }
                    }
                }

                candidatesPickList.Sort(new SelectedItemComparer());
            }

            // Return next pick (if the list has recently been created, the first element will be returned)
            int unselectIndex = lastCandidatePicked;
            lastCandidatePicked++;
            if (lastCandidatePicked >= candidatesPickList.Count)
                lastCandidatePicked = 0;

            int count;
            if ((count = candidatesPickList.Count) > 0)
            {
                // Unselect previously selected Item
                if (unselectIndex >= 0 && count > 1)
                    candidatesPickList[unselectIndex].item.IsSelected = false;
                return candidatesPickList[lastCandidatePicked].item;
            }
            else
                return null;
        }
        */
        #endregion

        /// <summary>
        /// Método para determinar lo objetos que se encuentren dentro de la ventana.
        /// Para ello se obtienen primero los parámetros (normal n y punto en el plano s)
        /// de 4 planos correspondientes a las 4 aristas de la ventana. Estos pueden ser 
        /// ortogonales o piramidales (perspectiva) según el tipo de proyección activo.
        /// 
        /// Las esquinas se nombran como sigue:
        ///     A----------B        (fx1,fy1)-----------(fx2,fy1)
        ///     |  World   |        |       Screen space        |
        ///     |  Space   |        |                           |
        ///     D----------C        (fx1,fy2)-----------(fx2,fy2)
        /// 
        /// Tomando las siguientes ecuaciones vectoriales:
        ///     X = P + rQ           (Ec. Recta)
        ///     0 = (X - S) . N      (Ec. Plano)
        /// 
        /// Se puede conocer la distancia entre un punto P y un plano si se crea una
        /// recta con base en P y dirección N y se interseca con el plano.
        /// 
        /// La intersección de una recta y un plano es como sigue:
        /// 
        ///     0 = (P + rQ - S) . N,
        /// 
        /// y si la dirección de la recta es perpendicular al plano, tenemos que:
        /// 
        ///     Q = N
        /// 
        /// lo que nos lleva a:
        ///         
        ///     r = (P - S) . N/|N| = (P.(N/|N|)) - (S.(N/|N|))
        /// 
        /// Si N es un vector unitario, entonces:
        /// 
        ///     N = N / |N|
        /// 
        /// Por lo tanto:
        /// 
        ///     r = P.N - S.N
        /// 
        /// y la distancia entre P y el plano es igual a r.
        /// 
        ///     P.N es la distancia desde el punto al origen, perpendicular al plano
        ///     posX es P.N con N = la normal al plano BC
        ///     posY es P.N con N = la normal al plano AB
        ///     deltaX es la distancia entre los planos BC y DA
        ///     deltaY es la distancia entre los planos AB y CD
        /// 
        /// Si N apunta hacia afuera en los 4 planos mencionados arriba, entonces una r
        /// positiva corresponde a estar en la parte interna del plano (con respecto a la
        /// caja formada por los 4). Por lo tanto un punto con las 4 r's positivas será un
        /// punto dentro de dicha caja.
        /// </summary>
        /// <param name="activeView">La vista (ventana) activa</param>
        internal void SelectWindow(Canguro.View.GraphicView activeView, int fx1, int fy1, int fx2, int fy2, List<Canguro.Model.Item> selectedItems, Controller.SelectionFilter selectionFilter)
        {
            int idJ1, idJ2, idJ3, idJ4;
            Canguro.Model.Model model = Canguro.Model.Model.Instance;

            #region Get the selection Box Params
            Vector3 nAB, sAB, nBC, sBC, nCD, sCD, nDA, sDA;
            Vector3[] corners1, corners2;
            corners1 = new Vector3[4];
            corners2 = new Vector3[4];

            // Checar si el cuadro está vacío
            if (fx1 == fx2 && fy1 == fy2)
                return;

            // Obtener parámetros de los 4 planos
            getPlanes(activeView, fx1, fy1, fx2, fy2, out nAB, out sAB, out nBC, out sBC, out nCD, out sCD, out nDA, out sDA, ref corners1, ref corners2);

            // Calcular los parámetros que dependen únicamente de los planos
            // (Como sólo nos interesa el signo de r y no su magnitud real, 
            // no es necesario calcular N.N)
            float snAB, snBC, snCD, snDA; // nnAB, nnBC, nnCD, nnDA;
            snAB = Vector3.Dot(sAB, nAB);
            snBC = Vector3.Dot(sBC, nBC);
            snCD = Vector3.Dot(sCD, nCD);
            snDA = Vector3.Dot(sDA, nDA);
            #endregion
            
            #region Displacements and Deformation factors
            float[,] displacements = null;
            float deformedScaleFactor = 0f;
            if (model.HasResults && model.Results.JointDisplacements != null)
            {
                displacements = model.Results.JointDisplacements;
                if (activeView.ModelRenderer.RenderOptions.ShowDeformed)
                {
                    deformedScaleFactor = model.Results.PaintScaleFactorTranslation;
                    if (activeView.ModelRenderer.RenderOptions.ShowAnimated)
                        deformedScaleFactor *= activeView.ModelRenderer.RenderOptions.DeformationScale;
                }
            }
            #endregion

            #region Joints select window/crossing (Cohen-Sutherland classification)
            Vector3 jPos;
            float posY, posX;
            float distY, distX;

            distY = (float)-(snCD + snAB);
            distX = (float)-(snBC + snDA);

            #region Array memory management
            // Array to qualify all the joints as in the Cohen Sutherland Algorithm
            // See http://www.cs.helsinki.fi/group/goa/viewing/leikkaus/lineClip.html for more info
            int jointCount = model.JointList.Count;
            int newBufferLength = jointPos.Length;
            if (newBufferLength != jointCount)
            {
                while (jointCount > newBufferLength)
                    newBufferLength *= 2;

                jointPos = new byte[newBufferLength];
                jointPosY = new float[newBufferLength];
                jointPosX = new float[newBufferLength];
            }
            #endregion

            // Determinar los Joints que se encuentren dentro de la caja, si es lo que se pide
            foreach (Canguro.Model.Joint j in model.JointList)
            {
                if (j != null)
                {
                    idJ1 = (int)j.Id;
                    jPos = getJointPos(j, displacements, deformedScaleFactor);
                    jointPosY[idJ1] = posY = Vector3.Dot(jPos, nAB);
                    jointPosX[idJ1] = posX = Vector3.Dot(jPos, nBC);

                    #region Cohen Sutherland classification
                    // Se divide la pantalla de la siguiente forma:
                    // 1001 | 1000 | 1010
                    // 0001 | 0000 | 0010
                    // 0101 | 0100 | 0110
                    //
                    // Si el puntoA OR puntoB == 0 está dentro
                    // Si el puntoA AND puntoB != 0 está fuera
                    // Si no se cumple ninguna de las anteriores, hay que checar la intersección para ver si está dentro
                    if (snAB <= posY)
                    {
                        if (snBC <= posX)
                        {
                            if (snCD <= (- posY)) // posCD
                            {
                                if (snDA <= (- posX)) // posDA
                                {
                                    // Está dentro --> seleccionar
                                    if (j.IsVisible && ((selectionFilter & Canguro.Controller.SelectionFilter.Joints) > 0))
                                    {
                                        if (selectedItems == null)
                                        {
                                            j.IsSelected = true;
                                            newlySelected = true;
                                        }
                                        else
                                            selectedItems.Add(j);
                                    }

                                    jointPos[idJ1] = 0x0;
                                }
                                else
                                    jointPos[idJ1] = 0x8;
                            }
                            else
                            {
                                if (snDA <= (- posX)) // posDA
                                    jointPos[idJ1] = 0x2;
                                else
                                    jointPos[idJ1] = 0xA;
                            }
                        }
                        else
                        {
                            if (snCD <= (- posY)) // posCD
                                jointPos[idJ1] = 0x4;
                            else
                                jointPos[idJ1] = 0x6;
                        }
                    }
                    else
                    {
                        if (snDA >= (- posX)) // posDA
                            jointPos[idJ1] = 0x9;
                        else if (snBC <= posX)
                            jointPos[idJ1] = 0x1;
                        else
                            jointPos[idJ1] = 0x5;
                    }
                    #endregion
                }
            }
            #endregion

            if (crossingSelectionOn || selectedItems != null)
            {
                #region Crossing Selection

                #region Lines crossing selection
                if ((selectionFilter & Canguro.Controller.SelectionFilter.Lines) > 0)
                {
                    // Selección de LineElements tipo Select Crossing
                    foreach (Canguro.Model.LineElement l in model.LineList)
                    {
                        if (l != null && l.IsVisible)
                        {
                            idJ1 = (int)l.I.Id;
                            idJ2 = (int)l.J.Id;
                            if ((jointPos[idJ1] & jointPos[idJ2]) == 0)
                            {
                                if (jointPos[idJ1] == 0 || jointPos[idJ2] == 0 || (jointPos[idJ1] == 0x1 && jointPos[idJ2] == 0x2) || (jointPos[idJ1] == 0x2 && jointPos[idJ2] == 0x1) || (jointPos[idJ1] == 0x8 && jointPos[idJ2] == 0x4) || (jointPos[idJ1] == 0x4 && jointPos[idJ2] == 0x8))
                                {
                                    if (selectedItems == null)
                                    {
                                        l.IsSelected = true;
                                        newlySelected = true;
                                    }
                                    else
                                        selectedItems.Add(l);
                                }
                                else
                                {
                                    // Check if line is inside
                                    float yL, yR;
                                    yL = (jointPosY[idJ1] - snAB) +
                                        ((snBC - jointPosX[idJ1]) / (jointPosX[idJ2] - jointPosX[idJ1])) * (jointPosY[idJ2] - jointPosY[idJ1]);
                                    yR = (jointPosY[idJ1] - snAB) +
                                        ((snDA + jointPosX[idJ1]) / (jointPosX[idJ1] - jointPosX[idJ2])) * (jointPosY[idJ2] - jointPosY[idJ1]);

                                    if ((yL >= 0 && yR <= distY) || (yL <= distY && yR >= 0))
                                    {
                                        if (selectedItems == null)
                                        {
                                            l.IsSelected = true;
                                            newlySelected = true;
                                        }
                                        else
                                            selectedItems.Add(l);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Areas crossing selection
                if ((selectionFilter & Canguro.Controller.SelectionFilter.Areas) > 0)
                {
                    Vector3 cornersDir = Vector3.Normalize(corners2[0] - corners1[0]);

                    // Selección de AreaElements tipo Select Crossing
                    foreach (Canguro.Model.AreaElement a in model.AreaList)
                    {
                        if (a != null && a.IsVisible)
                        {
                            idJ1 = (int)a.J1.Id;
                            idJ2 = (int)a.J2.Id;
                            idJ3 = (int)a.J3.Id;
                            idJ4 = (int)a.J4.Id;

                            if ((jointPos[idJ1] & jointPos[idJ2] & jointPos[idJ3] & jointPos[idJ4]) == 0)
                            {
                                if (jointPos[idJ1] == 0 || jointPos[idJ2] == 0 || jointPos[idJ3] == 0 || jointPos[idJ4] == 0 || 
                                    ((jointPos[idJ1] | jointPos[idJ2]) == 0x3) || ((jointPos[idJ1] | jointPos[idJ3]) == 0x3) || ((jointPos[idJ1] | jointPos[idJ4]) == 0x3) || ((jointPos[idJ2] | jointPos[idJ3]) == 0x3) || ((jointPos[idJ2] | jointPos[idJ4]) == 0x3) || ((jointPos[idJ3] | jointPos[idJ4]) == 0x3) ||
                                    ((jointPos[idJ1] | jointPos[idJ2]) == 0xC) || ((jointPos[idJ1] | jointPos[idJ3]) == 0xC) || ((jointPos[idJ1] | jointPos[idJ4]) == 0xC) || ((jointPos[idJ2] | jointPos[idJ3]) == 0xC) || ((jointPos[idJ2] | jointPos[idJ4]) == 0xC) || ((jointPos[idJ3] | jointPos[idJ4]) == 0xC))
                                {
                                    if (selectedItems == null)
                                    {
                                        a.IsSelected = true;
                                        newlySelected = true;
                                    }
                                    else
                                        selectedItems.Add(a);
                                }
                                else
                                {
                                    // Check if area intersect selection box

                                    Vector3 p, u, v, w, rst;
                                    p = a.J1.Position;
                                    u = a.J2.Position - p;
                                    v = a.J3.Position - p;
                                    w = Vector3.Scale(cornersDir, -1f);

                                    bool cornerInside = isCornerInsideTriangle(ref p, ref u, ref v, ref w, ref corners1);
                                    if (!cornerInside && a.J4 != null)
                                    {
                                        u = a.J4.Position - p;
                                        cornerInside = isCornerInsideTriangle(ref p, ref u, ref v, ref w, ref corners1);
                                    }

                                    if (cornerInside)
                                    {
                                        if (selectedItems == null)
                                        {
                                            a.IsSelected = true;
                                            newlySelected = true;
                                        }
                                        else
                                            selectedItems.Add(a);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #endregion
            }
            else
            {                           
                #region Window Selection
                // Line select window
                if ((selectionFilter & Canguro.Controller.SelectionFilter.Lines) > 0)
                {
                    // Selección de LineElements tipo Select Window
                    foreach (Canguro.Model.LineElement l in model.LineList)
                    {
                        if (l != null && l.IsVisible)
                        {
                            if (jointPos[(int)l.I.Id] + jointPos[(int)l.J.Id] == 0)
                            {
                                if (selectedItems == null)
                                {
                                    l.IsSelected = true;
                                    newlySelected = true;
                                }
                                else
                                    selectedItems.Add(l);
                            }
                        }
                    }
                }

                // Areas select window
                if ((selectionFilter & Canguro.Controller.SelectionFilter.Areas) > 0)
                {
                    // Selección de LineElements tipo Select Window
                    foreach (Canguro.Model.AreaElement a in model.AreaList)
                    {
                        if (a != null && a.IsVisible)
                        {
                            int sumJPos = jointPos[(int)a.J1.Id] + jointPos[(int)a.J2.Id] + jointPos[(int)a.J3.Id] + ((a.J4 != null) ? jointPos[(int)a.J4.Id] : (byte)0);
                            if (sumJPos == 0)
                            {
                                if (selectedItems == null)
                                {
                                    a.IsSelected = true;
                                    newlySelected = true;
                                }
                                else
                                    selectedItems.Add(a);
                            }
                        }
                    }
                }
                #endregion
            }

        }


        /// <summary>
        /// Check if the ray of each corner of the selection box intersects the area
        /// Let the area can be divided in two triangles T1 and T2, and the corners
        /// of the selection window be named C1, C2, C3 and C4.
        /// The equation for the planes defined by T1 and T2 is P + rU + sV = X
        /// where P is any point in the triangle, and U and V are the vectors forming the triangle's edges
        /// The equation for each corner will be Q - tW = X
        /// where Q is any point on the ray and W is the direction vector (perpendicular to the screen)
        /// Q's are obtained from corners1 and W from cornersDir
        ///
        /// This solution involves solving the following linear system of equations:
        /// | U0   V0   W0 |   | r |
        /// | U1   V1   W1 | x | s | = (Q - P)
        /// | U2   V2   W2 |   | t |
        /// </summary>
        /// <param name="p">Any point on the plane</param>
        /// <param name="u">A vector joining two vertices of a triangle</param>
        /// <param name="v">Another vector joining two vertices of a triangle (must be differecnt that u)</param>
        /// <param name="w">Viewing direction vector</param>
        /// <param name="qs">Array of points to be ckecked if inside the triangle</param>
        /// <returns>True if any point in qs lies inside the triangle. False otherwise</returns>
        private bool isCornerInsideTriangle(ref Vector3 p, ref Vector3 u, ref Vector3 v, ref Vector3 w, ref Vector3[] qs)
        {
            Vector3 rst;

            float det = u.X * (w.Z * v.Y - v.Z * w.Y) -
                        u.Y * (w.Z * v.X - v.Z * w.X) +
                        u.Z * (w.Y * v.X - v.Y * w.X);

            // If plane and ray are parallel
            if (det < Utility.GeometricUtils.Epsilon && det > -Utility.GeometricUtils.Epsilon)
                return false;

            det = 1f / det;
            Matrix inv = new Matrix();
            inv.M11 = det * (w.Z * v.Y - v.Z * w.Y); inv.M21 = -det * (w.Z * v.X - v.Z * w.X); inv.M31 = det * (w.Y * v.X - v.Y * w.X);
            inv.M12 = -det * (w.Z * u.Y - u.Z * w.Y); inv.M22 = det * (w.Z * u.X - u.Z * w.X); inv.M32 = -det * (w.Y * u.X - u.Y * w.X);
            inv.M13 = det * (v.Z * u.Y - u.Z * v.Y); inv.M23 = -det * (v.Z * u.X - u.Z * v.X); inv.M33 = det * (v.Y * u.X - u.Y * v.X);
            inv.M44 = 1f;

            for (int i = 0; i < qs.Length; i++)
            {
                rst = Vector3.TransformCoordinate(qs[i] - p, inv);
                if ((rst.X > 0) && (rst.Y > 0) && ((rst.X + rst.Y) < 1))
                    return true;
            }

            return false;
        }

        private Vector3 getJointPos(Canguro.Model.Joint j, float[,] displacements, float scalefactor)
        {
            if (displacements == null)
                return j.Position;
            else
            {
                int id = (int)j.Id;
                Vector3 deformation = new Vector3(displacements[id, 0], displacements[id, 1], displacements[id, 2]);
                return j.Position + Vector3.Scale(deformation, scalefactor);
            }
        }

        /// <summary>
        /// Method to return the four bounding planes given two points in screen coordinates 
        /// (as in two mouse clicks). A plane can be defined by a point on it and its normal,
        /// which is what is beaing returned for each of the four planes as defined by selectWindow.
        /// 
        /// For this, 8 points are calculated, two for each corner, for the front and back plane.
        /// This defines a box. The points are to be defined as pointing towards the screen (i.e. a1 lies
        /// in the middle of the volume and a2 is in the front plane), regardless of the coordinate system
        /// used (left or right handed), and the second point will be at the screen plane.
        /// </summary>
        /// <param name="activeView">The view that's used for calculating the bounding planes</param>
        /// <param name="fx1">Clicked first point, X screen coordinate</param>
        /// <param name="fy1">Clicked first point, Y screen coordinate</param>
        /// <param name="fx2">Clicked second point, X screen coordinate</param>
        /// <param name="fy2">Clicked second point, Y screen coordinate</param>
        /// <param name="nAB">Returns the calculated normal for the plane AB</param>
        /// <param name="sAB">Returns a point on the calculated plane AB</param>
        /// <param name="nBC">Returns the calculated normal for the plane BC</param>
        /// <param name="sBC">Returns a point on the calculated plane BC</param>
        /// <param name="nCD">Returns the calculated normal for the plane CD</param>
        /// <param name="sCD">Returns a point on the calculated plane CD</param>
        /// <param name="nDA">Returns the calculated normal for the plane DA</param>
        /// <param name="sDA">Returns a point on the calculated plane DA</param>
        private void getPlanes(Canguro.View.GraphicView activeView, int fx1, int fy1, int fx2, int fy2,
                                out Vector3 nAB, out Vector3 sAB, out Vector3 nBC, out Vector3 sBC, 
                                out Vector3 nCD, out Vector3 sCD, out Vector3 nDA, out Vector3 sDA, 
                                ref Vector3[] corners1, ref Vector3[] corners2)
        {
            // First calculate the 8 points (2 for each corner)
            Vector3 a1 = new Vector3(fx1, fy1, 0.5f);
            Vector3 a2 = new Vector3(fx1, fy1, 1);
            Vector3 b1 = new Vector3(fx2, fy1, 0.5f);
            Vector3 b2 = new Vector3(fx2, fy1, 1);
            Vector3 c1 = new Vector3(fx2, fy2, 0.5f);
            Vector3 c2 = new Vector3(fx2, fy2, 1);
            Vector3 d1 = new Vector3(fx1, fy2, 0.5f);
            Vector3 d2 = new Vector3(fx1, fy2, 1);
            activeView.Unproject(ref a1);
            activeView.Unproject(ref a2);
            activeView.Unproject(ref b1);
            activeView.Unproject(ref b2);
            activeView.Unproject(ref c1);
            activeView.Unproject(ref c2);
            activeView.Unproject(ref d1);
            activeView.Unproject(ref d2);

            if (corners1 != null && corners2 != null && corners1.Length == 4 && corners2.Length == 4)
            {
                corners1[0] = a1;
                corners1[1] = b1;
                corners1[2] = c1;
                corners1[3] = d1;

                corners2[0] = a2;
                corners2[1] = b2;
                corners2[2] = c2;
                corners2[3] = d2;
            }

            // Now calculate the normals
            // If points P and Q are consecutive and in CCW order, then
            // normal = (q2 - p1) x (q1 - p1)
            nAB = Vector3.Normalize(Vector3.Cross((b2 - a1), (b1 - a1)));
            nBC = Vector3.Normalize(Vector3.Cross((c2 - b1), (c1 - b1)));
            nCD = Vector3.Normalize(Vector3.Cross((d2 - c1), (d1 - c1)));
            nDA = Vector3.Normalize(Vector3.Cross((a2 - d1), (a1 - d1)));

            // Now assign the points on the planes.
            // Points calculated at the corners are used here
            sAB = a1;
            sBC = c1;
            sCD = c1;
            sDA = a1;
        }
        #endregion

        #region ViewCommand Selection Utilities
        public bool IsWorkingAsCommandService
        {
            get { return (services != null); }
        }

        public Vector3 GetViewableObjectsCentroid(Canguro.View.GraphicView activeView)
        {
            // Set window equal to viewport to select everything visible
            int fx1 = activeView.Viewport.X;
            int fy1 = activeView.Viewport.Y;
            int fx2 = activeView.Viewport.X + activeView.Viewport.Width;
            int fy2 = activeView.Viewport.Y + activeView.Viewport.Height;

            List<Canguro.Model.Item> crossingItems = new List<Canguro.Model.Item>();
            SelectWindow(activeView, fx1, fy1, fx2, fy2, crossingItems, Canguro.Controller.SelectionFilter.Joints | Canguro.Controller.SelectionFilter.Lines);

            int centroidPts = 0;
            Vector3 centroid = Vector3.Empty;

            foreach (Canguro.Model.Item item in crossingItems)
            {
                if (item is Canguro.Model.Joint)
                {
                    centroidPts++;
                    centroid += ((Canguro.Model.Joint)item).Position;
                }
                else if (item is Canguro.Model.LineElement)
                {
                    centroidPts++;
                    centroid += ((Canguro.Model.LineElement)item).I.Position;
                    centroidPts++;
                    centroid += ((Canguro.Model.LineElement)item).J.Position;
                }
            }

            if (centroidPts > 0)
                return centroid * (1.0f / centroidPts);
            else
            {
                //if nothing is in view, then return the center of the viewing frustum
                Vector3 screenCenter = new Vector3(activeView.Viewport.X + activeView.Viewport.Width / 2, activeView.Viewport.Y + activeView.Viewport.Height / 2, 0.5f);
                activeView.Unproject(ref screenCenter);
                return screenCenter;
            }
        }
        #endregion
    }
}
