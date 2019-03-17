using Dos2.ModManager.Commands;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Dos2.ModManager.ViewModels
{
    /// <summary>
    /// SUMMARY: viewmodel for wcc lite workflows
    /// TODO serialize as xmls
    /// 
    /// </summary>
    public class WorkspaceViewModel : DockableViewModel, IDropTarget
    {
        /// <summary>
        /// File path to the to-serialize xml
        /// </summary>
        #region FilePath
        private string _filePath;
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    InvokePropertyChanged();
                    InvokePropertyChanged(nameof(Title));
                }
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        #region Title
        public override string Title
        {
            get
            {
                if (String.IsNullOrEmpty(FilePath))
                {
                    return base.Title;
                }
                if (HasUnsavedChanges)
                {
                    return String.Format("{0}*", Path.GetFileName(FilePath));
                }
                return Path.GetFileName(FilePath);
            }
        }
       
        #endregion

        #region HasUnsavedChanges
        private bool _hasUnsavedChanges;
        public bool HasUnsavedChanges
        {
            get
            {
                return _hasUnsavedChanges;
            }
            set
            {
                if (_hasUnsavedChanges != value)
                {
                    _hasUnsavedChanges = value;
                    InvokePropertyChanged();
                    InvokePropertyChanged(nameof(Title));
                }
            }
        }
        #endregion

        #region IsSelected
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    InvokePropertyChanged();
                }
            }
        }
        #endregion




        public ICommand CloseCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }

        public WorkspaceViewModel()
        {
            CloseCommand = new RelayCommand(Close);
            SaveCommand = new RelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);

            //Workflow = new ObservableCollection<string>();
        }

        private void SaveAs()
        {

        }

        private void Save()
        {
            HasUnsavedChanges = false;
        }

        private void Close()
        {

        }


       

        //FIXME add more checks when I add more droppable lists.
        public void DragOver(IDropInfo dropInfo)
        {
            object sourceItem = dropInfo.Data;
            var targetItem = dropInfo.TargetItem;
            IDragInfo dragInfo = dropInfo.DragInfo;
            var sourceCollection = dragInfo.SourceCollection;
            var targetCollection = dropInfo.TargetCollection;

            if (sourceItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
                if (targetCollection == sourceCollection) // drag and drop within one list
                    dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            IDragInfo dragInfo = dropInfo.DragInfo;
            object sourceItem = dropInfo.Data;
            object targetItem = dropInfo.TargetItem;
            
            /*
            if (dropInfo.Effects == DragDropEffects.Move) //if drag and drop *within* the list
            {
                var sourceCollection = dropInfo.DragInfo.SourceCollection.Cast<WccCommand>().ToArray(); // UNSAFE if not castable to list
                var insertIndex = dropInfo.InsertIndex;
                var sourceIndex = dropInfo.DragInfo.SourceIndex;
                insertIndex = Math.Min(insertIndex, sourceCollection.Length - 1);

                Workflow.Move(sourceIndex, insertIndex);
            }
            else //handle all dropping *into* the list
            {
                // handle dropping of commands
                if (sourceItem.GetType().BaseType == typeof(WccCommand))
                {
                    WccCommand emptyCopy = (WccCommand)Activator.CreateInstance(sourceItem.GetType());
                    Workflow.Add(emptyCopy);
                }
                // handle dropping of variables 
                else if (sourceItem.GetType() == typeof(WccUIVariable) && targetItem.GetType() == typeof(string)) 
                {
                    
                }    
            }
                */

        }
    }
}
