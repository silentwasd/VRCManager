﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WorldManager.Views.SavedWorlds;

public partial class SavedSelectionView : UserControl
{
    public SavedSelectionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}