using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using HarfBuzzSharp;
using HotAvalonia;

namespace ArtSigner;

public partial class MainWindow : Window
{
    private string name_of_working_directory;
    private Uri path_of_working_directory;
    private Queue<string> images;
    private WorkerWindow worker;

    public MainWindow()
    {
        InitializeComponent();
        this.Setup();
    }

    private void Setup()
    {
        this.worker = new WorkerWindow();
        // throw new NotImplementedException();
    }

    public async void GetWorkingDirectory(object sender, RoutedEventArgs args)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Your Art Folder"
        });
        if (folders.Count < 1)
        {
            return;
        }
        this.name_of_working_directory = folders[0].Name;
        this.path_of_working_directory = folders[0].Path;
        if (sender is Button clickedbutton)
        {
            clickedbutton.Content = name_of_working_directory;
        }
        this.CreateImageQueue();
    }

    public void CreateImageQueue()
    {
        string[] files = Directory.GetFiles(this.path_of_working_directory.AbsolutePath);

        var images = from file in files
                     where file.Contains(".png") || file.Contains(".jpg")
                     select file;

        Queue<string> images_to_generate = new Queue<string>(images.ToArray());

        this.images = images_to_generate;
        this.update_counter();
    }

    public void update_counter()
    {
        var number = this.images.Count();
        this.NumberOfImages.Text = $"{number} images will be processed";
    }

    public async void StartGenerating(object? sender, RoutedEventArgs args)
    {
        // WorkerWindow worker = 
        // worker.Show();
        this.update_counter();
        if (worker is { IsVisible: true })
        {
            worker.Close();
        }

        worker = new WorkerWindow();
        worker.Show(this);

        worker.SetTextContent(this.NameTextBox.Text);
        worker.load_images(images.ToList());
        worker.working_directory = this.path_of_working_directory;
        await worker.RunAsync();
        this.NumberOfImages.Text += "\nCompleted!";

    }


}