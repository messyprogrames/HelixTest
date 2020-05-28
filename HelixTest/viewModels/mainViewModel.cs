// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using HelixToolkit.Wpf.SharpDX;
using PropertyChanged;
using Color = System.Windows.Media.Color;
using Color4 = SharpDX.Color4;
using Colors = System.Windows.Media.Colors;
using Element3D = HelixToolkit.Wpf.SharpDX.Element3D;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3 = SharpDX.Vector3;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using SharpDX.Direct3D11;
using M3 = System.Windows.Media.Media3D;
using Material = HelixToolkit.Wpf.SharpDX.Material;
using SharpDX;

namespace HelixTest
{
    public class mainViewModel : baseViewModel
    {
        public ObservableCollection<Element3D> selection { get; set; } = new ObservableCollection<Element3D>();
        public Vector3 selectionCenterOffset { get; set; }
        public Color selectionColor { get; private set; }
        public Material selectionMaterial { get; private set; }

        #region UI Properties

        public bool isPanelIdActive { get; set; }
        public bool isPanelPropertiesSaveActive { get; set; }
        public IEnumerable<projectMaterials> helixMaterials { get; private set; }

        #endregion

        #region selected panel(s) properties

        public string selectedPanelId { get; set; }
        public projectMaterials selectedPanelMaterial { get; set; }
        public double selectedPanelMaterialOrientation { get; set; }

        #endregion

        #region IO properties

        public string filepathCSV { get; set; }

        #endregion
        
        public Vector3D upDirection { set; get; } = new Vector3D(0, 0, 1);

        public LineGeometry3D majorGrid { get; private set; }
        public Color majorGridColor { get; private set; }
        public Transform3D majorGridTransform { get; private set; }

        public LineGeometry3D panelGrid { get; private set; }
        public Color panelGridColor { get; private set; }
        public Transform3D panelGridTransform { get; private set; }

        public ObservableCollection<cubeDataModel> cubes { get; private set; }

        public Color directionalLightColor { get; set; }
        public Color ambientLightColor { get; set; }

        public Camera camera2d { get; private set; }
        public Vector3D upDirection2d { set; get; } = new Vector3D(0, 1, 0);

        //! COMMANDS
        public ICommand updateProperties { get; private set; }
        public ICommand changePreviewMaterial { get; private set; }

        //! VISIBILITY BOOLS
        public bool showCurves { get; set; }
        public bool showFaces { get; set; }
        public bool showPanels { get; set; }

        public PerspectiveCamera Camera
        {
            get { return this.camera as PerspectiveCamera; }

            set
            {
                camera = value;
                //RaisePropertyChanged("Camera");
            }
        }


        /// <summary>
        /// constructor
        /// </summary>
        public mainViewModel()
        {

            showCurves = true;
            showFaces = false;
            showPanels = true;

            effectsManager = new DefaultEffectsManager();

            helixMaterials = Enum.GetValues(typeof(projectMaterials)).Cast<projectMaterials>();

            updateProperties = new relayCommand(x => updatePanelProperties(selection));
            changePreviewMaterial = new relayCommand(x => previewSelectionMaterial());

            // titles
            title = "App";
            subTitle = "Test";

            // camera setup
            camera = new PerspectiveCamera
            {
                Position = new Point3D(1500, 1500, 2500),
                LookDirection = new Vector3D(-60, -60, -100),
                UpDirection = upDirection,
                FarPlaneDistance = 1e4,
                NearPlaneDistance = 0.01,
            };

            camera2d = new OrthographicCamera
            {
                Position = new Point3D(0, 0, 55),
                LookDirection = new Vector3D(0, 0, -25),
                UpDirection = upDirection2d,
                FarPlaneDistance = 1e4,
                NearPlaneDistance = 0.01,
            };

            // setup lighting
            ambientLightColor = Colors.DimGray;
            directionalLightColor = Colors.White;

            // selection options
            //selectionColor = Color.FromRgb(91, 198, 208);
            selectionColor = Colors.Red;
            selectionMaterial = new DiffuseMaterial() {DiffuseColor = new Color4(91, 198, 208, 255)};

            // floor plane grid (1')
            majorGrid = LineBuilder.GenerateGrid(new Vector3(1, 1, 0), -100, 100, -100, 100);
            majorGridColor = Color.FromArgb(150, 200, 200, 200);
            majorGridTransform = new Media3D.TranslateTransform3D(0, 0, -0.01);

            // grid (1')
            panelGrid = LineBuilder.GenerateGrid(new Vector3(1, 1, 0), -10, 10, -10, 10);
            panelGridColor = Color.FromArgb(255, 100, 100, 100);
            panelGridTransform = new Media3D.TranslateTransform3D(0, 0, -0.01);

            addCubes();
        }


        /// <summary>
        /// Gather data from helixRevitHelper for display in Helix viewer.
        /// </summary>
        public void addCubes()
        {

            IEnumerable<int> count = Enumerable.Range(0, 5);
            IEnumerable<int> newCenters = count.Select(x => x * 5);

            IList<cubeDataModel> cubesToAdd = new List<cubeDataModel>();
            foreach (int i in newCenters)
            {
                Vector3 center = new Vector3(i, 0, 0);
                cubesToAdd.Add(new cubeDataModel(center, 3));
            }

            cubes = new ObservableCollection<cubeDataModel>();
            foreach (cubeDataModel sdm in cubesToAdd) cubes.Add(sdm);
        }


        #region panel properties functions

        /// <summary>
        /// Changes the Panel Properties UI to reflect any changes to the selected panels
        /// </summary>
        private void updateSelectionProperties()
        {
            isPanelIdActive = true;
            isPanelPropertiesSaveActive = true;
            // if there's anything but panels in the list, return nothing
            if (selection.Any(x => x.DataContext.GetType() != typeof(cubeDataModel)))
            {
                isPanelIdActive = false;
                isPanelPropertiesSaveActive = false;
                selectedPanelId = "(non-panels selected)";
                selectedPanelMaterial = projectMaterials.None;
            }
            // if there is not selection, return nothing
            else if (selection == null || selection.Count == 0)
            {
                selectedPanelId = "Select a Panel";
                selectedPanelMaterial = projectMaterials.None;
            }
            else
            {
                // convert to list of panel data models
                List<cubeDataModel> pdms = new List<cubeDataModel>();
                foreach (Element3D geo in selection) pdms.Add(geo.DataContext as cubeDataModel);
                
                // check if all have the same panel material
                if (pdms.Count != pdms.Where(p => p.material == pdms.First().material).ToList().Count) selectedPanelMaterial = projectMaterials.None;
                else selectedPanelMaterial = pdms[0].material;

            }
        }


        /// <summary>
        /// Pushes changes made to panel properties in the UI to the panels
        /// </summary>
        private void updatePanelProperties(IEnumerable<Element3D> currentSelection)
        {
            foreach (Element3D geo in currentSelection)
            {
                // get the panel data model
                cubeDataModel pdm = geo.DataContext as cubeDataModel;
                // change the panel data model's properties
                if (selectedPanelMaterial != projectMaterials.None) pdm.material = selectedPanelMaterial;


            }
        }

        #endregion


        #region selection functions

        /// <summary>
        /// Get element that is hit on mouse down in 3d viewport.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onMouseDown3DHandler(object sender, MouseDown3DEventArgs e)
        {
            MouseEventArgs mouseArgs = e.OriginalInputEventArgs as MouseEventArgs;

            // check if the shift key is held down
            bool shiftDown = false;
            KeyStates state1 = Keyboard.GetKeyStates(Key.LeftShift);
            KeyStates state2 = Keyboard.GetKeyStates(Key.RightShift);
            if ((state1 & KeyStates.Down) > 0 || (state2 & KeyStates.Down) > 0) shiftDown = true;

            // only pay attention if its a LEFT click
            if (mouseArgs.LeftButton == MouseButtonState.Pressed)
            {
                if (e.HitTestResult != null && e.HitTestResult.ModelHit is GeometryModel3D geo)
                {
                    // if shift is held, add to the selection
                    if (shiftDown)
                    {
                        //selectionCenterOffset = geo.Geometry.Bound.Center; // must update before updating selection
                        selection.Add(geo);
                        updateSelectionProperties();
                        previewSelectionMaterial();
                        highlightSelection();
                    }
                    // else change selection to only what was clicked
                    else
                    {
                        clearSelection();
                        //selectionCenterOffset = geo.Geometry.Bound.Center; // must update before updating selection
                        selection.Add(geo);
                        updateSelectionProperties();
                        previewSelectionMaterial();
                        highlightSelection();
                    }

                }
                // if they clicked nothing
                else
                {
                    if (!shiftDown)
                    {
                        clearSelection();
                        updateSelectionProperties();
                    }
                }
            }
        }

        /// <summary>
        /// Highlight selected object.
        /// </summary>
        private void highlightSelection()
        {
            foreach (Element3D ele in selection)
            {
                Type objectType = ele.GetType();
                // highlight meshes
                if (objectType == typeof(MeshGeometryModel3D))
                {
                    MeshGeometryModel3D meshSel = ele as MeshGeometryModel3D;
                    meshSel.PostEffects = "borderHighlight";
                } 
            }
        }
        /// <summary>
        /// Remove object highlight after deselection.
        /// </summary>
        /// <param name="oldSelection"></param>
        private void removeHighlight(Element3D oldSelection)
        {
            // remove highlight from meshes
            MeshGeometryModel3D meshSel = oldSelection as MeshGeometryModel3D;
            meshSel.PostEffects = null;

        }
        /// <summary>
        /// Perform all actions for clearing the selection - clearing panel lines, selection list, remove post effect
        /// </summary>
        public void clearSelection()
        {
            foreach (Element3D element in selection)
            {
                // remove highglight
                removeHighlight(element);
                // revert material
                revertMaterial(element);
            }

            selectionCenterOffset = new Vector3(0, 0, 0);
            selection.Clear();
        }


        /// <summary>
        /// Reverts any temporary material previews upon deselection
        /// </summary>
        private void revertMaterial(Element3D oldSelection)
        {
            // remove any temp preview colors
            Type objectType = oldSelection?.GetType();
            if (objectType == typeof(MeshGeometryModel3D))
            {
                MeshGeometryModel3D meshSel = oldSelection as MeshGeometryModel3D;
                if (oldSelection.DataContext.GetType() == typeof(cubeDataModel))
                {
                    // get the pdm
                    cubeDataModel pdm = oldSelection.DataContext as cubeDataModel;
                    // reset the color
                    meshSel.Material = projectMatToHelixMatConverter.Convert(pdm.material) as Material;
                }

            }
        }

        /// <summary>
        /// previews the panel's material or any temp materials used in the materials dropdown
        /// </summary>
        private void previewSelectionMaterial()
        {
            foreach (Element3D element in selection)
            {
                if (element.GetType() == typeof(CompositeModel3D))
                {
                    CompositeModel3D cm = element as CompositeModel3D;
                    IEnumerable<MeshGeometryModel3D> meshes = cm.Children.Where(x => x.GetType() == typeof(MeshGeometryModel3D)).Cast<MeshGeometryModel3D>();
                    foreach (MeshGeometryModel3D mesh in meshes)
                    {
                        //mesh.DataContext = element.DataContext;
                        previewElementMaterial(mesh);
                    }
                }
                else if (element.GetType() == typeof(MeshGeometryModel3D)) previewElementMaterial(element as MeshGeometryModel3D);
            }
        }

        /// <summary>
        /// previews the material of a particular element.
        /// used within the previewSelectionMaterial function
        /// </summary>
        private void previewElementMaterial(MeshGeometryModel3D element)
        {
            // get the mesh
            MeshGeometryModel3D meshSel = element as MeshGeometryModel3D;
            // get data context
            if (element.DataContext.GetType() == typeof(cubeDataModel))
            {
                // get the pdm
                cubeDataModel pdm = element.DataContext as cubeDataModel;
                // assign the color
                //pdm.panelMaterial = (selectedPanelMaterial != projectMaterials.None) ? selectedPanelMaterial : pdm.panelMaterial;
                meshSel.Material = (selectedPanelMaterial != projectMaterials.None) ? projectMatToHelixMatConverter.Convert(selectedPanelMaterial) as Material : projectMatToHelixMatConverter.Convert(pdm.material) as Material;
            }

        }

        #endregion

    }
}