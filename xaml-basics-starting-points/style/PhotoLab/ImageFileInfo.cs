﻿//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoLab
{
    public class ImageFileInfo : INotifyPropertyChanged
    {
        public ImageFileInfo(ImageProperties properties, StorageFile imageFile,
            BitmapImage src, string name, string type)
        {
            ImageProperties = properties;
            ImageSource = src;
            ImageName = name;
            ImageFileType = type;
            ImageFile = imageFile;
            var rating = (int)properties.Rating;
            var random = new Random();
            ImageRating = rating == 0 ? random.Next(1, 5) : rating;
        }

        public StorageFile ImageFile { get; }

        public ImageProperties ImageProperties { get; }

        private BitmapImage _imageSource = null;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public string ImageName { get; }

        public string ImageFileType { get; }

        public string ImageDimensions => $"{ImageSource.PixelWidth} x {ImageSource.PixelHeight}";

        public string ImageTitle
        {
            get => String.IsNullOrEmpty(ImageProperties.Title) ? ImageName : ImageProperties.Title;
            set
            {
                if (ImageProperties.Title != value)
                {
                    ImageProperties.Title = value;
                    var ignoreResult = ImageProperties.SavePropertiesAsync();
                    OnPropertyChanged();
                }
            }
        }

        public int ImageRating
        {
            get => (int)ImageProperties.Rating;
            set
            {
                if (ImageProperties.Rating != value)
                {
                    ImageProperties.Rating = (uint)value;
                    var ignoreResult = ImageProperties.SavePropertiesAsync();
                    OnPropertyChanged();
                }
            }
        }

        private float _exposure = 0;
        public float Exposure
        {
            get => _exposure;
            set => SetEditingProperty(ref _exposure, value);
        }

        private float _temperature = 0;
        public float Temperature
        {
            get => _temperature;
            set => SetEditingProperty(ref _temperature, value);
        }

        private float _tint = 0;
        public float Tint
        {
            get => _tint;
            set => SetEditingProperty(ref _tint, value);
        }

        private float _contrast = 0;
        public float Contrast
        {
            get => _contrast;
            set => SetEditingProperty(ref _contrast, value);
        }

        private float _saturation = 1;
        public float Saturation
        {
            get => _saturation;
            set => SetEditingProperty(ref _saturation, value);
        }

        private float _blur = 0;
        public float Blur
        {
            get => _blur;
            set => SetEditingProperty(ref _blur, value);
        }

        private bool _needsSaved = false; 
        public bool NeedsSaved
        {
            get => _needsSaved;
            set => SetProperty(ref _needsSaved, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetEditingProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                if (Exposure != 0 || Temperature != 0 || Tint != 0 || Contrast != 0 || Saturation != 1 || Blur != 0)
                {
                    NeedsSaved = true;
                }
                else
                {
                    NeedsSaved = false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }
    }
}
