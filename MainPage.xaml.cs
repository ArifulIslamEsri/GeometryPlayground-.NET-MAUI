using System.Collections.Generic;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Maui;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;

// ✅ Fix naming conflict
using Map = Esri.ArcGISRuntime.Mapping.Map;
using DrawingColor = System.Drawing.Color;

namespace GeometryPlayground;

public partial class MainPage : ContentPage
{
    enum Mode { None, Add, Delete, Line, Polygon }

    private Mode currentMode = Mode.None;

    private GraphicsOverlay overlay = new();
    private List<MapPoint> points = new();

    public MainPage()
    {
        InitializeComponent();

        MyMapView.Map = new Map(BasemapStyle.ArcGISNavigation);
        MyMapView.GraphicsOverlays.Add(overlay);

        MyMapView.GeoViewTapped += OnMapTapped;
    }

    private async void OnMapTapped(object? sender, GeoViewInputEventArgs e)
    {
        var point = e.Location;
        if (point == null) return;

        switch (currentMode)
        {
            case Mode.Add:
                overlay.Graphics.Add(new Graphic(
                    point,
                    new SimpleMarkerSymbol(
                        SimpleMarkerSymbolStyle.Circle,
                        DrawingColor.Red,
                        10)));
                break;

            case Mode.Delete:
                var result = await MyMapView.IdentifyGraphicsOverlaysAsync(
                    e.Position, 10, false);

                if (result.Count > 0 && result[0].Graphics.Count > 0)
                    overlay.Graphics.Remove(result[0].Graphics[0]);
                break;

            case Mode.Line:
                points.Add(point);
                DrawLine();
                break;

            case Mode.Polygon:
                points.Add(point);
                DrawPolygon();
                break;
        }
    }

    private void DrawLine()
    {
        overlay.Graphics.Clear();

        if (points.Count < 2) return;

        var builder = new PolylineBuilder(points[0].SpatialReference);

        foreach (var p in points)
            builder.AddPoint(p);

        overlay.Graphics.Add(new Graphic(
            builder.ToGeometry(),
            new SimpleLineSymbol(
                SimpleLineSymbolStyle.Solid,
                DrawingColor.Blue,
                3)));
    }

    private void DrawPolygon()
    {
        overlay.Graphics.Clear();

        if (points.Count < 3) return;

        var builder = new PolygonBuilder(points[0].SpatialReference);

        foreach (var p in points)
            builder.AddPoint(p);

        overlay.Graphics.Add(new Graphic(
            builder.ToGeometry(),
            new SimpleFillSymbol(
                SimpleFillSymbolStyle.Solid,
                DrawingColor.FromArgb(80, 0, 255, 0),
                new SimpleLineSymbol(
                    SimpleLineSymbolStyle.Solid,
                    DrawingColor.Green,
                    2))));
    }

    // ✅ BUTTON HANDLERS

    private void OnAddClicked(object sender, EventArgs e)
    {
        currentMode = Mode.Add;
        points.Clear();
    }

    private void OnDeleteClicked(object sender, EventArgs e)
    {
        currentMode = Mode.Delete;
        points.Clear();
    }

    private void OnLineClicked(object sender, EventArgs e)
    {
        currentMode = Mode.Line;
        points.Clear();
        overlay.Graphics.Clear();
    }

    private void OnPolygonClicked(object sender, EventArgs e)
    {
        currentMode = Mode.Polygon;
        points.Clear();
        overlay.Graphics.Clear();
    }

    private void OnFinishClicked(object sender, EventArgs e)
    {
        currentMode = Mode.None;
        points.Clear();
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        overlay.Graphics.Clear();
        points.Clear();
        currentMode = Mode.None;
    }
}