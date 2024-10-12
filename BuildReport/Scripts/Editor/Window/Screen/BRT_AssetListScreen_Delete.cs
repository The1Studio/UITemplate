using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BuildReportTool.Window.Screen
{
    public partial class AssetList
    {
        private bool ShouldShowDeleteButtons(BuildInfo buildReportToDisplay)
        {
            return
                (this.IsShowingUnusedAssets && buildReportToDisplay.HasUnusedAssets) || (buildReportToDisplay.HasUsedAssets && BuildReportTool.Options.AllowDeletingOfUsedAssets);
        }

        private void InitiateDeleteSelectedUsed(BuildInfo buildReportToDisplay)
        {
            var listToDeleteFrom = this.GetAssetListToDisplay(buildReportToDisplay);

            this.InitiateDeleteSelectedInAssetList(buildReportToDisplay, listToDeleteFrom);
        }

        private void InitiateDeleteSelectedInAssetList(BuildInfo buildReportToDisplay, BuildReportTool.AssetList listToDeleteFrom)
        {
            if (listToDeleteFrom.IsNothingSelected) return;

            var all = listToDeleteFrom.All;

            var numOfFilesRequestedToDelete = listToDeleteFrom.GetSelectedCount();
            var numOfFilesToDelete          = numOfFilesRequestedToDelete;
            var systemDeletionFileCount     = 0;
            var brtFilesSelectedForDelete   = 0;

            // filter out files that shouldn't be deleted
            // and identify unrecoverable files
            for (int n = 0, len = all.Length; n < len; ++n)
            {
                var b                     = all[n];
                var isThisFileToBeDeleted = listToDeleteFrom.InSumSelection(b);

                if (isThisFileToBeDeleted)
                {
                    if (Util.IsFileInBuildReportFolder(b.Name) && !Util.IsUselessFile(b.Name))
                    {
                        //Debug.Log("BRT file? " + b.Name);
                        --numOfFilesToDelete;
                        ++brtFilesSelectedForDelete;
                    }
                    else if (Util.HaveToUseSystemForDelete(b.Name))
                    {
                        ++systemDeletionFileCount;
                    }
                }
            }

            if (numOfFilesToDelete <= 0)
            {
                if (brtFilesSelectedForDelete > 0)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Can't delete!",
                        "Take note that for safety, Build Report Tool assets themselves will not be included for deletion.",
                        "OK");
                }

                return;
            }

            // prepare warning message for user

            var deletingSystemFilesOnly = systemDeletionFileCount == numOfFilesToDelete;
            var deleteIsRecoverable     = !deletingSystemFilesOnly;

            var plural                         = "";
            if (numOfFilesToDelete > 1) plural = "s";

            string message;

            if (numOfFilesRequestedToDelete != numOfFilesToDelete)
                message = "Among " + numOfFilesRequestedToDelete + " file" + plural + " requested to be deleted, only " + numOfFilesToDelete + " will be deleted.";
            else
                message = "This will delete " + numOfFilesToDelete + " asset" + plural + " in your project.";

            // add warning about BRT files that are skipped
            if (brtFilesSelectedForDelete > 0) message += "\n\nTake note that for safety, " + brtFilesSelectedForDelete + " file" + (brtFilesSelectedForDelete > 1 ? "s" : "") + " found to be Build Report Tool assets are not included for deletion.";

            // add warning about unrecoverable files
            if (systemDeletionFileCount > 0)
            {
                if (deletingSystemFilesOnly)
                    message += "\n\nThe deleted file" + plural + " will not be recoverable from the " + Util.NameOfOSTrashFolder + ", unless you have your own backup.";
                else
                    message += "\n\nAmong the " + numOfFilesToDelete + " file" + plural + " for deletion, " + systemDeletionFileCount + " will not be recoverable from the " + Util.NameOfOSTrashFolder + ", unless you have your own backup.";

                message +=
                    "\n\nThis is a limitation in Unity and .NET code. To ensure deleting will move the files to the " + Util.NameOfOSTrashFolder + " instead, delete your files the usual way using your project view.";
            }
            else
            {
                message += "\n\nThe deleted file" + plural + " can be recovered from your " + Util.NameOfOSTrashFolder + ".";
            }

            message +=
                "\n\nDeleting a large number of files may take a long time as Unity will rebuild the project's Library folder.\n\nProceed with deleting?";

            EditorApplication.Beep();
            if (!EditorUtility.DisplayDialog("Delete?", message, "Yes", "No")) return;

            var allList  = new List<SizePart>(all);
            var toRemove = new List<SizePart>(all.Length / 4);

            // finally, delete the files
            var deletedCount = 0;
            for (int n = 0, len = allList.Count; n < len; ++n)
            {
                var b = allList[n];

                var okToDelete = Util.IsUselessFile(b.Name) || !Util.IsFileInBuildReportFolder(b.Name);

                if (listToDeleteFrom.InSumSelection(b) && okToDelete)
                {
                    // delete this

                    if (Util.ShowFileDeleteProgress(deletedCount,
                        numOfFilesToDelete,
                        b.Name,
                        deleteIsRecoverable))
                        return;

                    Util.DeleteSizePartFile(b);
                    toRemove.Add(b);
                    ++deletedCount;
                }
            }

            EditorUtility.ClearProgressBar();

            // refresh the asset lists
            allList.RemoveAll(i => toRemove.Contains(i));
            var allWithRemoved = allList.ToArray();

            // recreate per category list (maybe just remove from existing per category lists instead?)
            var perCategoryOfList =
                ReportGenerator.SegregateAssetSizesPerCategory(allWithRemoved,
                    buildReportToDisplay.FileFilters);

            listToDeleteFrom.Reinit(allWithRemoved,
                perCategoryOfList,
                this.IsShowingUsedAssets
                    ? BuildReportTool.Options.NumberOfTopLargestUsedAssetsToShow
                    : BuildReportTool.Options.NumberOfTopLargestUnusedAssetsToShow);
            listToDeleteFrom.ClearSelection();

            // print info about the delete operation to console
            var finalMessage                      = string.Format("{0} file{1} removed from your project.", deletedCount.ToString(), plural);
            if (deleteIsRecoverable) finalMessage += " They can be recovered from your " + Util.NameOfOSTrashFolder + ".";

            EditorApplication.Beep();
            EditorUtility.DisplayDialog("Delete successful", finalMessage, "OK");

            Debug.LogWarning(finalMessage);
        }

        private void InitiateDeleteAllUnused(BuildInfo buildReportToDisplay)
        {
            var list = buildReportToDisplay.UnusedAssets;
            var all  = list.All;

            var filesToDeleteCount = 0;

            for (int n = 0, len = all.Length; n < len; ++n)
            {
                var b = all[n];

                var okToDelete = Util.IsFileOkForDeleteAllOperation(b.Name);

                if (okToDelete)
                    //Debug.Log("added " + b.Name + " for deletion");
                    ++filesToDeleteCount;
            }

            if (filesToDeleteCount == 0)
            {
                const string NOTHING_TO_DELETE =
                    "Take note that for safety, Build Report Tool assets, Unity editor assets, version control metadata, and Unix-style hidden files will not be included for deletion.\n\nYou can force deleting them by selecting them (via the checkbox) and using \"Delete selected\", or simply delete them the normal way in your project view.";

                EditorApplication.Beep();
                EditorUtility.DisplayDialog("Nothing to delete!", NOTHING_TO_DELETE, "Ok");
                return;
            }

            var plural                         = "";
            if (filesToDeleteCount > 1) plural = "s";

            EditorApplication.Beep();
            if (!EditorUtility.DisplayDialog("Delete?",
                string.Format(
                    "Among {0} file{1} in your project, {2} will be deleted.\n\nBuild Report Tool assets themselves, Unity editor assets, version control metadata, and Unix-style hidden files will not be included for deletion. You can force-delete those by selecting them (via the checkbox) and use \"Delete selected\", or simply delete them the normal way in your project view.\n\nDeleting a large number of files may take a long time as Unity will rebuild the project's Library folder.\n\nAre you sure about this?\n\nThe file{1} can be recovered from your {3}.",
                    all.Length.ToString(),
                    plural,
                    filesToDeleteCount.ToString(),
                    Util.NameOfOSTrashFolder),
                "Yes",
                "No"))
                return;

            var newAll = new List<SizePart>();

            var deletedCount = 0;
            for (int n = 0, len = all.Length; n < len; ++n)
            {
                var b = all[n];

                var okToDelete = Util.IsFileOkForDeleteAllOperation(b.Name);

                if (okToDelete)
                {
                    // delete this
                    if (Util.ShowFileDeleteProgress(deletedCount, filesToDeleteCount, b.Name, true)) return;

                    Util.DeleteSizePartFile(b);
                    ++deletedCount;
                }
                else
                    //Debug.Log("added " + b.Name + " to new list");
                {
                    newAll.Add(b);
                }
            }

            EditorUtility.ClearProgressBar();

            var newAllArr = newAll.ToArray();

            var perCategoryUnused =
                ReportGenerator.SegregateAssetSizesPerCategory(newAllArr, buildReportToDisplay.FileFilters);

            list.Reinit(newAllArr,
                perCategoryUnused,
                this.IsShowingUsedAssets
                    ? BuildReportTool.Options.NumberOfTopLargestUsedAssetsToShow
                    : BuildReportTool.Options.NumberOfTopLargestUnusedAssetsToShow);
            list.ClearSelection();

            var finalMessage = string.Format(
                "{0} file{1} removed from your project. They can be recovered from your {2}.",
                filesToDeleteCount.ToString(),
                plural,
                Util.NameOfOSTrashFolder);
            Debug.LogWarning(finalMessage);

            EditorApplication.Beep();
            EditorUtility.DisplayDialog("Delete successful", finalMessage, "OK");
        }
    }
}