﻿using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DaxStudio.UI.Converters
{
    // Referenced by QueryResultsPaneView.xaml
    //
    public class DynamicDataGridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var columns = new ObservableCollection<DataGridColumn>();
            var dv = value as DataView;
            if (dv != null)
            {
                var dg = new DataGrid();

                //var gridView = new GridView();
                var cols = dv.ToTable().Columns;
                foreach (DataColumn item in cols)
                {
                    // This section turns off the RecogniseAccessKey setting in the column header
                    // which allows it to display underscores correctly.
                    var hdrTemplate = new DataTemplate();
                    var contentPresenter = new FrameworkElementFactory(typeof(Border));
                    contentPresenter.SetValue(ContentPresenter.RecognizesAccessKeyProperty,false);
                    var txtBlock = new FrameworkElementFactory(typeof(TextBlock));
                    
                    txtBlock.SetValue(TextBlock.TextProperty, item.Caption);
                    
                    contentPresenter.AppendChild(txtBlock);
                    hdrTemplate.VisualTree = contentPresenter;
                    
                    var cellTemplate = new DataTemplate();
                    if (item.DataType == typeof(Byte[]))
                    {
                        var style = new Style { TargetType = typeof(ToolTip) };
                        
                        //style.Setters.Add(new Setter { Property = TemplateProperty, Value = GetToolTip(dataTable) });
                        //style.Setters.Add(new Setter { Property = OverridesDefaultStyleProperty, Value = true });
                        //style.Setters.Add(new Setter { Property = System.Windows.Controls.ToolTip.HasDropShadowProperty, Value = true });
                        //Resources.Add(typeof(ToolTip), style);

                        var cellImgBlock = new FrameworkElementFactory(typeof(Image));
                        var cellTooltip = new FrameworkElementFactory(typeof(ToolTip));
                        var cellImgTooltip = new FrameworkElementFactory(typeof(Image));
                        cellImgTooltip.SetValue(Image.WidthProperty, 150d);
                        
                        cellImgBlock.SetValue(FrameworkContentElement.ToolTipProperty, cellTooltip);
                        cellTooltip.SetValue(ToolTip.ContentProperty, cellImgTooltip);

                        // Adding square brackets around the bind will escape any column names with the following "special" binding characters   . / ( ) [ ]
                        
                        cellImgBlock.SetBinding(Image.SourceProperty, new Binding("[" + item.ColumnName + "]"));
                        cellImgTooltip.SetBinding(Image.SourceProperty, new Binding("[" + item.ColumnName + "]"));
                        cellImgBlock.SetValue(Image.WidthProperty, 50d);
                        //cellImgBlock.SetValue(FrameworkElement.ToolTipProperty, cellImgTooltip);
                        
                        cellTemplate.VisualTree = cellImgBlock;
                    }
                    else
                    {
                        var cellTxtBlock = new FrameworkElementFactory(typeof(TextBlock));
                        // Adding square brackets around the bind will escape any column names with the following "special" binding characters   . / ( ) [ ]
                        var colBinding = new Binding("[" + item.ColumnName + "]");
                        cellTxtBlock.SetBinding(TextBlock.TextProperty, colBinding);
                        
                        cellTxtBlock.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
                        cellTxtBlock.SetBinding(FrameworkElement.ToolTipProperty, colBinding );
                        cellTemplate.VisualTree = cellTxtBlock;
                        
                    }
                    var dgc = new DataGridTemplateColumn
                    {
                        CellTemplate = cellTemplate,
                    //    Width = Double.NaN,    
                        HeaderTemplate = hdrTemplate,
                        Header = item.Caption,
                        
                        ClipboardContentBinding = new Binding(item.ColumnName)
                    };

                    columns.Add(dgc);
                    //dg.Columns.Add(gvc);
                }

                return columns;
            }
            return Binding.DoNothing;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

