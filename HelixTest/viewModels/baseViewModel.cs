// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Base ViewModel for Demo Applications?
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using HelixToolkit.Wpf.SharpDX.Model;

namespace HelixTest
{
    using HelixToolkit.Wpf.SharpDX;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Base ViewModel for Demo Applications?
    /// </summary>
    public abstract class baseViewModel : ObservableObject, IDisposable
    {
        public const string orthographic = "Orthographic Camera";

        public const string perspective = "Perspective Camera";

        private string _cameraModel;

        private Camera _camera;

        private string _subTitle;

        private string _title;

        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                Set(ref _title, value, "Title");
            }
        }

        public string subTitle
        {
            get
            {
                return _subTitle;
            }
            set
            {
                Set(ref _subTitle, value, "SubTitle");
            }
        }

        public List<string> cameraModelCollection { get; private set; }

        public string cameraModel
        {
            get
            {
                return _cameraModel;
            }
            set
            {
                if (Set(ref _cameraModel, value, "CameraModel"))
                {
                    onCameraModelChanged();
                }
            }
        }

        public Camera camera
        {
            get
            {
                return _camera;
            }

            protected set
            {
                Set(ref _camera, value, "Camera");
                cameraModel = value is PerspectiveCamera
                                       ? perspective
                                       : value is OrthographicCamera ? orthographic : null;
            }
        }

        private IEffectsManager _effectsManager;

        public IEffectsManager effectsManager
        {
            get { return _effectsManager; }
            protected set
            {
                Set(ref _effectsManager, value);
            }
        }

        protected OrthographicCamera defaultOrthographicCamera = new OrthographicCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 1, FarPlaneDistance = 100 };

        protected PerspectiveCamera defaultPerspectiveCamera = new PerspectiveCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 0.5, FarPlaneDistance = 150 };

        public event EventHandler cameraModelChanged;

        protected baseViewModel()
        {
            // camera models
            cameraModelCollection = new List<string>()
            {
                orthographic,
                perspective,
            };

            // on camera changed callback
            cameraModelChanged += (s, e) =>
            {
                if (_cameraModel == orthographic)
                {
                    if (!(camera is OrthographicCamera))
                        camera = defaultOrthographicCamera;
                }
                else if (_cameraModel == perspective)
                {
                    if (!(camera is PerspectiveCamera))
                        camera = defaultPerspectiveCamera;
                }
                else
                {
                    throw new HelixToolkitException("Camera Model Error.");
                }
            };

            // default camera model
            cameraModel = perspective;

            title = "Demo (HelixToolkitDX)";
            subTitle = "Default Base View Model";
        }

        protected virtual void onCameraModelChanged()
        {
            var eh = cameraModelChanged;
            if (eh != null)
            {
                eh(this, new EventArgs());
            }
        }

        public static MemoryStream loadFileToMemory(string filePath)
        {
            using (var file = new FileStream(filePath, FileMode.Open))
            {
                var memory = new MemoryStream();
                file.CopyTo(memory);
                return memory;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                if (effectsManager != null)
                {
                    var effectManager = effectsManager as IDisposable;
                    Disposer.RemoveAndDispose(ref effectManager);
                }
                disposedValue = true;
                GC.SuppressFinalize(this);
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~baseViewModel()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}