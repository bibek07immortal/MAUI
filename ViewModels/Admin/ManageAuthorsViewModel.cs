﻿using BookNest.Models;
using BookNest.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BookNest.ViewModels.Admin
{
    public partial class ManageAuthorsViewModel : ObservableObject
    {

        #region PROPERTIES

        // Snack Bar

        [ObservableProperty]
        private float _snackBarOpacity = 0;

        [ObservableProperty]
        private string _snackBarMessage = string.Empty;

        //

        private string textSearch = string.Empty;
        public string TextSearched
        {
            get { return textSearch; }
            set
            {
                textSearch = value;
                OnPropertyChanged(nameof(TextSearched));
                LoadAuthors();
            }
        }

        [ObservableProperty]
        private ObservableCollection<Author> authors = new();

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private bool isPopupVisible = false;

        [ObservableProperty]
        private bool isAddEditAuthorVisible = false;

        [ObservableProperty]
        private bool isDeleteAuthorVisible = false;

        [ObservableProperty]
        private string addEditAuthorHeading = string.Empty;

        #endregion

        private bool isAddAuthor = false;
        private Author? tempAuthor = null;

        public ManageAuthorsViewModel()
        {
            LoadAuthors();
        }


        #region COMMANDS

        [RelayCommand]
        private void AddAuthor()
        {
            AddEditAuthorHeading = "Add Author";
            Name = string.Empty;
            IsPopupVisible = true;
            IsAddEditAuthorVisible = true;
            isAddAuthor = true;
        }


        [RelayCommand]
        private void EditAuthor(Author author)
        {
            tempAuthor = author;
            Name = tempAuthor.Name;
            AddEditAuthorHeading = "Edit Author";
            IsPopupVisible = true;
            IsAddEditAuthorVisible = true;
            tempAuthor = author;
        }

        [RelayCommand]
        private void AddEditConfirm()
        {
            if (!Validation.IsValidName(Name))
            {
                ShowSnackBar("Invalid name");
                return;
            }

            if (isAddAuthor)
            {
                tempAuthor = new Author { Name = Name };
                isAddAuthor = false;
            }
            else
            {
                if(tempAuthor != null)
                {
                    tempAuthor.Name = Name;
                }
            }
            App.AuthorsRepo.SaveItem(tempAuthor);
            LoadAuthors();
            IsPopupVisible = false;
            IsAddEditAuthorVisible = false;
        }

        [RelayCommand]
        private void DeleteAuthor(Author author)
        {
            if (IsDeleteAuthorVisible)
            {
                App.AuthorsRepo.DeleteItem(tempAuthor);
                IsPopupVisible = false;
                IsDeleteAuthorVisible = false;
                LoadAuthors();
                return;
            }
            tempAuthor = author;
            IsPopupVisible = true;
            IsDeleteAuthorVisible = true;
        }

        [RelayCommand]
        private void DeleteCancel()
        {
            IsPopupVisible = false;
            IsDeleteAuthorVisible = false;
        }

        [RelayCommand]
        private void ClosePopup()
        {
            IsPopupVisible = false;
            IsAddEditAuthorVisible = false;
            IsDeleteAuthorVisible = false;
            isAddAuthor = false;
            Name = string.Empty;
        }
        #endregion


        private void LoadAuthors()
        {
            Authors.Clear();
            var filteredAuthors = string.IsNullOrWhiteSpace(TextSearched)
                ? App.AuthorsRepo.GetItemsWithChildren()
                : App.AuthorsRepo.GetItemsWithChildren()
                    .Where(a => a.Name.Contains(TextSearched, StringComparison.OrdinalIgnoreCase));

            foreach (var book in filteredAuthors)
            {
                Authors.Add(book);
            }
        }

        private async void ShowSnackBar(string message)
        {
            SnackBarMessage = message;


            for (float i = 0.0f; i <= 0.9f; i += 0.1f)
            {
                SnackBarOpacity = i;
                await Task.Delay(50);
            }

            await Task.Delay(4000);


            for (float i = 0.9f; i >= 0.0f; i -= 0.1f)
            {
                SnackBarOpacity = i;
                await Task.Delay(50);
            }
            SnackBarOpacity = 0;
        }
    }
}
