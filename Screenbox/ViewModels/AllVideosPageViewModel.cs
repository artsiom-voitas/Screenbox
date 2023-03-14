﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Screenbox.Controls;
using Screenbox.Core;
using Screenbox.Core.Messages;
using Screenbox.Services;
using System.Collections.Generic;

namespace Screenbox.ViewModels
{
    internal sealed partial class AllVideosPageViewModel : ObservableRecipient
    {
        public ObservableCollection<MediaViewModel> Videos { get; }

        private readonly ILibraryService _libraryService;

        public AllVideosPageViewModel(ILibraryService libraryService)
        {
            _libraryService = libraryService;
            Videos = new ObservableCollection<MediaViewModel>();
        }

        public async Task FetchVideosAsync()
        {
            IReadOnlyList<MediaViewModel> videos = await _libraryService.FetchVideosAsync();
            Videos.Clear();
            foreach (MediaViewModel video in videos)
            {
                Videos.Add(video);
            }
        }

        [RelayCommand]
        private void Play(MediaViewModel media)
        {
            if (Videos.Count == 0) return;
            PlaylistInfo playlist = Messenger.Send(new PlaylistRequestMessage());
            if (playlist.Playlist.Count != Videos.Count || playlist.LastUpdate != Videos)
            {
                Messenger.Send(new ClearPlaylistMessage());
                Messenger.Send(new QueuePlaylistMessage(Videos, false));
            }

            Messenger.Send(new PlayMediaMessage(media, true));
        }

        [RelayCommand]
        private void PlayNext(MediaViewModel media)
        {
            Messenger.SendPlayNext(media);
        }

        [RelayCommand]
        private async Task ShowPropertiesAsync(MediaViewModel media)
        {
            ContentDialog propertiesDialog = PropertiesView.GetDialog(media);
            await propertiesDialog.ShowAsync();
        }
    }
}