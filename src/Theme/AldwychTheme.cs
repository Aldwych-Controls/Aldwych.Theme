using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

#nullable enable

namespace Aldwych.Theme
{
    public enum AldwychThemeMode
    {
        Light,
        Dark,
    }

    /// <summary>
    /// Includes the Aldwych theme in an application.
    /// </summary>
    public class AldwychTheme : IStyle, IResourceProvider
    {
        private readonly Uri _baseUri;
        private IStyle[]? _loaded;
        private bool _isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="AldwychTheme"/> class.
        /// </summary>
        /// <param name="baseUri">The base URL for the XAML context.</param>
        public AldwychTheme(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AldwychTheme"/> class.
        /// </summary>
        /// <param name="serviceProvider">The XAML service provider.</param>
        public AldwychTheme(IServiceProvider serviceProvider)
        {
            _baseUri = ((IUriContext)serviceProvider.GetService(typeof(IUriContext))).BaseUri;
        }

        /// <summary>
        /// Gets or sets the mode of the Aldwych theme (light, dark).
        /// </summary>
        public AldwychThemeMode Mode { get; set; }

        public IResourceHost? Owner => (Loaded as IResourceProvider)?.Owner;

        /// <summary>
        /// Gets the loaded style.
        /// </summary>
        public IStyle Loaded
        {
            get
            {
                if (_loaded == null)
                {
                    _isLoading = true;
                    var loaded = (IStyle)AvaloniaXamlLoader.Load(GetUri(), _baseUri);
                    _loaded = new[] { loaded };
                    _isLoading = false;
                }

                return _loaded?[0]!;
            }
        }

        bool IResourceNode.HasResources => (Loaded as IResourceProvider)?.HasResources ?? false;

        IReadOnlyList<IStyle> IStyle.Children => _loaded ?? Array.Empty<IStyle>();

        public event EventHandler OwnerChanged
        {
            add
            {
                if (Loaded is IResourceProvider rp)
                {
                    rp.OwnerChanged += value;
                }
            }
            remove
            {
                if (Loaded is IResourceProvider rp)
                {
                    rp.OwnerChanged -= value;
                }
            }
        }

        public SelectorMatchResult TryAttach(IStyleable target, IStyleHost? host) => Loaded.TryAttach(target, host);

        public bool TryGetResource(object key, out object? value)
        {
            if (!_isLoading && Loaded is IResourceProvider p)
            {
                return p.TryGetResource(key, out value);
            }

            value = null;
            return false;
        }

        void IResourceProvider.AddOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.AddOwner(owner);
        void IResourceProvider.RemoveOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.RemoveOwner(owner);

        private Uri GetUri() => Mode switch
        {
            AldwychThemeMode.Dark => new Uri("avares://Aldwych.Theme/AldwychDark.xaml", UriKind.Absolute),
            _ => new Uri("avares://Aldwych.Theme/AldwychLight.xaml", UriKind.Absolute),
        };
    }
}
