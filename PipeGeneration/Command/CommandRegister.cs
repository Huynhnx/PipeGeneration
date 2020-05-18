using Bricscad.ApplicationServices;
using Bricscad.EditorInput;
using Bricscad.Windows;
using PipeGeneration.GUI;
using PipeGeneration.Palette;
using PipeGeneration.ViewModel;
using System;
using System.Windows.Controls;
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.Runtime;

namespace PipeGeneration
{
    public class CommandRegister
    {
        [CommandMethod("PipeGenerate")]
        public static void PipeGenerate()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            TypedValue[] filList = new TypedValue[1] { new TypedValue((int)DxfCode.Start, "Line") };

            SelectionFilter filter = new SelectionFilter(filList);

            PromptSelectionOptions opts = new PromptSelectionOptions();

            opts.MessageForAdding = "Select Line to Generate Pipe: ";
            PromptSelectionResult res = ed.GetSelection(opts, filter);
            if (res.Status != PromptStatus.OK)
            {
                return;
            }

            if (res.Value.Count != 0)
            {
                SelectionSet set = res.Value;
                ObjectId[] Ids = set.GetObjectIds();
                double pipeOD = 10;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in Ids)
                    {
                        Line line = tr.GetObject(id, OpenMode.ForWrite) as Line;
                        Point3d ptstart = line.StartPoint;
                        Point3d ptEnd = line.EndPoint;
                        Circle circle = new Circle(ptstart, ptEnd - ptstart, pipeOD);
                        Solid3d solid = new Solid3d();
                        DBObjectCollection acDBObjColl = new DBObjectCollection();
                        acDBObjColl.Add(circle);
                        DBObjectCollection myRegionColl = new DBObjectCollection();
                        myRegionColl = Region.CreateFromCurves(acDBObjColl);
                        Region acRegion = myRegionColl[0] as Region;
                        solid.ExtrudeAlongPath(acRegion,line,0);
                        AppendEntity(solid);

                    }
                    tr.Commit();
                }
            }
        }
        [CommandMethod("SettingPalette")]
        public void SettingPalette()
        {
            SettingPalette ps = SettingPaletteSingleton.Instance;
            if (SettingPaletteSingleton.IsCreateNew)
            {
                if (ps != null)
                {
                    //                    ps.PaletteSetClosed += PaletteSet_PaletteSetClosed;
                }
                SettingPaletteSingleton.IsCreateNew = false;
            }
            SettingViewModel vm = new SettingViewModel();
            vm.routingfromLinesCmd = new MVVMCore.RelayCommand(RoutingFromLinesInvoke);
            if (ps != null)
            {
                UserControl currentUser = SettingPaletteSingleton.getTabAt(1);
                if (currentUser == null)
                {
                    SettingView view = new SettingView();
                    view.DataContext = vm;

                    ps.AddVisual("Setting", view); // TRANS             
                    SettingPaletteSingleton.addTab(1, view);
                    ps.Text = "Setting"; ; // TRANS
                    ps.Name = "Setting"; // TRANS
                    ps.Visible = true;
                    ps.Size = new System.Drawing.Size(500, 600);
                    ps.Dock = DockSides.Left;
                }
                else
                {
                    currentUser.DataContext = vm;

                    ps.Visible = true;
                }
                UserControl tab2 = SettingPaletteSingleton.getTabAt(2);
                if (tab2 == null)
                {
                    SettingView view = new SettingView();
                    view.DataContext = vm;

                    ps.AddVisual("Communications", view); // TRANS             
                    SettingPaletteSingleton.addTab(2, view);

                    ps.Visible = true;
                    ps.Size = new System.Drawing.Size(500, 600);
                    ps.Dock = DockSides.Left;
                }
                else
                {
                    currentUser.DataContext = vm;

                    ps.Visible = true;
                }

            }
        }

        private void RoutingFromLinesInvoke(object obj)
        {
            SettingView view = obj as SettingView;
            if (view == null)
            {
                return;
            }
            SettingViewModel vm = view.DataContext as SettingViewModel;
            if (vm == null)
            {
                return;
            }
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            TypedValue[] filList = new TypedValue[1] { new TypedValue((int)DxfCode.Start, "Line") };

            SelectionFilter filter = new SelectionFilter(filList);

            PromptSelectionOptions opts = new PromptSelectionOptions();

            opts.MessageForAdding = "Select Line to Generate Pipe: ";
            PromptSelectionResult res = ed.GetSelection(opts, filter);
            if (res.Status != PromptStatus.OK)
            {
                return;
            }

            if (res.Value.Count != 0)
            {
                SelectionSet set = res.Value;
                ObjectId[] Ids = set.GetObjectIds();
                double pipeOD = vm.ProfileSelected.pipeSize;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in Ids)
                    {
                        Line line = tr.GetObject(id, OpenMode.ForWrite) as Line;
                        Point3d ptstart = line.StartPoint;
                        Point3d ptEnd = line.EndPoint;
                        Circle circle = new Circle(ptstart, ptEnd - ptstart, pipeOD);
                        Solid3d solid = new Solid3d();
                        DBObjectCollection acDBObjColl = new DBObjectCollection();
                        acDBObjColl.Add(circle);
                        DBObjectCollection myRegionColl = new DBObjectCollection();
                        myRegionColl = Region.CreateFromCurves(acDBObjColl);
                        Region acRegion = myRegionColl[0] as Region;
                        solid.ExtrudeAlongPath(acRegion, line, 0);
                        AppendEntity(solid);

                    }
                    tr.Commit();
                }
            }
        }

        public static ObjectId AppendEntity(Entity ent)
        {
            ObjectId objId = ObjectId.Null;
            if (ent == null)
            {
                return objId;
            }
            Document doc = Application.DocumentManager.MdiActiveDocument;
            try
            {
                using (doc.LockDocument())
                {
                    Database db = doc.Database;
                    // start new transaction
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        // open model space block table record
                        BlockTableRecord spaceBlkTblRec = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                        // append entity to model space block table record
                        objId = spaceBlkTblRec.AppendEntity(ent);
                        trans.AddNewlyCreatedDBObject(ent, true);
                        // finish
                        trans.Commit();
                    }
                }

            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage(ex.ToString());
            }
            return objId;
        }
    }
}
