using AgolPlugin.Models.Agol;
using AgolPlugin.Models.Common;
using AgolPlugin.ViewModels.Import;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgolPlugin.Services.Acad
{
    public class AcadWriter
    {
        private AcadService _acad;

        public AcadWriter(AcadService acad)
        {
            _acad = acad;
        }

        public Dictionary<LayerConfiguratorViewModel, ObjectId> LayersFromConfigs(IEnumerable<LayerConfiguratorViewModel> configs, Action onIncrement)
        {
            var db = _acad.Database;
            var dict = new Dictionary<LayerConfiguratorViewModel, ObjectId>();

            using (var lt = _acad.Transaction.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable)
            {
                foreach (var config in configs)
                {
                    ObjectId objId;
                    if (!lt.Has(config.AcadLayerName))
                    {
                        var newLayer = new LayerTableRecord
                        {
                            Name = config.AcadLayerName,
                            Color = config.AcadLayerColor,
                        };
                        objId = lt.Add(newLayer);
                        _acad.Transaction.AddNewlyCreatedDBObject(newLayer, true);
                    }
                    else
                    {
                        objId = lt[config.AcadLayerName];
                    }
                    dict[config] = objId;
                    onIncrement();
                }
            }
            return dict;
        }

        public Dictionary<LayerConfiguratorViewModel, ObjectId> BlockDefsFromConfigs(IEnumerable<LayerConfiguratorViewModel> configs, Action onIncrement)
        {
            var db = _acad.Database;
            var dict = new Dictionary<LayerConfiguratorViewModel, ObjectId>();

            using (var bt = _acad.Transaction.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable)
            {
                foreach (var config in configs)
                {
                    IList<FieldAttributeMapping> sortedMappings;
                    switch (config.PropertySortType)
                    {
                        case PropertySortType.None:
                            sortedMappings = config.Mappings;
                            break;
                        case PropertySortType.AliasAscending:
                            sortedMappings = config.Mappings.OrderBy(m => m.Field.Alias).ToList();
                            break;
                        case PropertySortType.AliasDescending:
                            sortedMappings = config.Mappings.OrderByDescending(m => m.Field.Alias).ToList();
                            break;
                        case PropertySortType.NameAscending:
                            sortedMappings = config.Mappings.OrderBy(m => m.Field.Name).ToList();
                            break;
                        case PropertySortType.NameDescending:
                            sortedMappings = config.Mappings.OrderByDescending(m => m.Field.Name).ToList();
                            break;
                    }

                    if (!bt.Has(config.NewAcadBlockName))
                    {
                        using (var blockDef = new BlockTableRecord())
                        {
                            blockDef.Name = config.NewAcadBlockName;
                            blockDef.Origin = Point3d.Origin;

                            using (var center = new DBPoint(Point3d.Origin))
                            {
                                center.Color = config.NewAcadBlockColor;
                                blockDef.AppendEntity(center);

                                foreach (var mapping in config.Mappings)
                                {
                                    if (mapping.Include)
                                    {
                                        var attribDef = new AttributeDefinition
                                        {
                                            Layer = config.AcadLayerName,
                                            Verifiable = false,
                                            Prompt = mapping.Field.Alias,
                                            Tag = mapping.Field.Name,
                                            OwnerId = blockDef.ObjectId,
                                        };

                                        if (mapping.UseAsBlockLabel)
                                        {
                                            var pnt = new Point3d(0, 10, 0);
                                            attribDef.Height = 10;
                                            attribDef.Position = pnt;
                                            attribDef.Justify = AttachmentPoint.BottomCenter;
                                            attribDef.Visible = true;
                                            attribDef.Invisible = false;
                                            attribDef.Color = config.NewAcadBlockColor;
                                            attribDef.AlignmentPoint = pnt;
                                        }
                                        else
                                        {
                                            attribDef.Height = 4;
                                            attribDef.Position = new Point3d(0, 0, 0);
                                            attribDef.Justify = AttachmentPoint.BaseLeft;
                                            attribDef.Invisible = true;
                                        }
                                        blockDef.AppendEntity(attribDef);
                                    }
                                }
                                var objId = bt.Add(blockDef);
                                _acad.Transaction.AddNewlyCreatedDBObject(blockDef, true);

                                config.BlockDefObjectId = objId;
                                dict[config] = objId;
                            }
                        }
                    }
                    onIncrement();
                }
            }
            return dict;
        }

        public Dictionary<AgolRecord, ObjectId> AddBlockReferences(IEnumerable<LayerConfiguratorViewModel> configs, Action onIncrement)
        {
            var db = _acad.Database;
            var dict = new Dictionary<AgolRecord, ObjectId>();

            using (var bt = _acad.Transaction.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable)
            {
                var ms = bt[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite) as BlockTableRecord;
                foreach (var config in configs)
                    foreach (var record in config.Feature.Records)
                    {
                        var blockDef = bt[config.NewAcadBlockName].GetObject(OpenMode.ForWrite) as BlockTableRecord;
                        using (var blockRef = new BlockReference(record.Geometry?.ToPoint3d() ?? Point3d.Origin, blockDef.ObjectId))
                        {
                            blockRef.Layer = config.AcadLayerName;
                            dict.Add(record, ms.AppendEntity(blockRef));
                            _acad.Transaction.AddNewlyCreatedDBObject(blockRef, true);

                            foreach (ObjectId objId in blockDef)
                            {
                                var dbObj = _acad.Transaction.GetObject(objId, OpenMode.ForRead);
                                if (dbObj is AttributeDefinition attDef && !attDef.Constant)
                                {
                                    var mapping = record[attDef.Tag].Field.Mapping;
                                    if (mapping.Include)
                                    {
                                        using (var attRef = new AttributeReference())
                                        {
                                            attRef.SetAttributeFromBlock(attDef, blockRef.BlockTransform);
                                            attRef.Position = attDef.Position.TransformBy(blockRef.BlockTransform);
                                            attRef.Tag = mapping.Field.Name;
                                            attRef.TextString = record[mapping.Field.Name].Value;

                                            blockRef.AttributeCollection.AppendAttribute(attRef);
                                            _acad.Transaction.AddNewlyCreatedDBObject(attRef, true);
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                            }

                            onIncrement?.Invoke();
                        }
                    }
            }
            return dict;
        }



        public void SetPdmodeAndSize(int pdmode = 34, int pdsize = 10)
        {
            _acad.Database.Pdmode = pdmode;
            _acad.Database.Pdsize = pdsize;
        }
    }
}