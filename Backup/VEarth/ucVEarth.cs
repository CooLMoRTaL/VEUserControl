using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;  
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using VEarth.Directions;
using VEarth.Locations;  
using VEarth.Shapes;

using JThomas.Tools;  

namespace VEarth
{
    public enum VE_Shape
    {
        Polyline = 0,
        Polygon = 1
    }

    #region Event handler Delegates
    public delegate void FinishedFindingLocations();
    public delegate void MouseClick(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey, bool leftButton, bool rightButton);
    public delegate void MouseDoubleClick(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey);
    public delegate void MouseMove(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey, bool leftButton, bool rightButton);
    public delegate void MouseWheel(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey);
    public delegate void MouseOver(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey);
    public delegate void MouseOut(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey);
    #endregion

    [ComVisible(true)]
    public partial class ucVEarth : UserControl
    {
        #region Private properties
        private int searchLocationIndex = 0;
        private bool centerAtLastResult = false;
        private bool displayResults = false;
        private bool finishedFindingLocations = false;
        private List<SearchLocation> locations = new List<SearchLocation>();
        private RouteDirections routeDirections = null;
        #endregion

        #region Eventhandler definitions
        public event FinishedFindingLocations VE_OnFinishedFindingLocations;
        public event MouseClick VE_OnMouseClick;
        public event MouseDoubleClick VE_OnMouseDoubleClick;
        public event MouseMove VE_OnMouseMove;
        public event MouseWheel VE_OnMouseWheel;
        public event MouseOver VE_OnMouseOver;
        public event MouseOut VE_OnMouseOut;
        #endregion

        #region Enumerations
        /// <summary>
        /// Enum of handling possibilities for disambiguatious search results
        /// </summary>
        public enum DisambiguationEnum
        {
            Default = 1,
            Ignore = 0
        }

        /// <summary>
        /// Enum of possible route optimizations when getting driving directions
        /// </summary>
        public enum RouteOptimizeEnum
        {
            Time = 0,
            Distance = 1
        }

        /// <summary>
        /// Enum of available distinance units
        /// </summary>
        public enum DistanceUnitEnum
        {
            Kilometers = 0,
            Miles = 1
        }

        /// <summary>
        /// Enum of available route modes
        /// </summary>
        public enum RouteModeEnum
        {
            Walking = 0,
            Driving = 1
        }

        /// <summary>
        /// Enum of available map styles
        /// </summary>
        public enum MapStyleEnum
        {
            Road = 0,
            Aerial = 1,
            Birdseye = 2,
            Hybrid = 3
        }

        /// <summary>
        /// Enum of available dashboard sizes
        /// </summary>
        public enum DashboardStyleEnum
        {
            Tiny = 0,
            Small = 1,
            Normal = 2
        }
        #endregion

        #region Javascript event handlers functions
        public void onclick(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey, bool leftButton, bool rightButton)
        {
            if (VE_OnMouseClick != null)
            {
                VE_OnMouseClick(latitude, longitude, id, altKey, ctrlKey, shiftKey, leftButton, rightButton);
            }
        }

        public void ondoubleclick(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey)
        {
            if (VE_OnMouseDoubleClick != null)
            {
                VE_OnMouseDoubleClick(latitude, longitude, id, altKey, ctrlKey, shiftKey);
            }
        }

        public void onmousemove(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey, bool leftButton, bool rightButton)
        {
            if (VE_OnMouseMove != null)
            {
                VE_OnMouseMove(latitude, longitude, id, altKey, ctrlKey, shiftKey, leftButton, rightButton);
            }
        }

        public void onmousewheel(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey)
        {
            if (VE_OnMouseWheel != null)
            {
                VE_OnMouseWheel(latitude, longitude, id, altKey, ctrlKey, shiftKey);
            }
        }

        public void onmouseout(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey)
        {
            if (VE_OnMouseOut != null)
            {
                VE_OnMouseOut(latitude, longitude, id, altKey, ctrlKey, shiftKey);
            }
        }

        public void onmouseover(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey)
        {
            if (VE_OnMouseOver != null)
            {
                VE_OnMouseOver(latitude, longitude, id, altKey, ctrlKey, shiftKey);
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ucVEarth()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the web browser control
        /// </summary>
        public void InitMap()
        {
            if (System.IO.File.Exists(htmlLocation))
            {
                vEarthBrowser.ObjectForScripting = this;
                vEarthBrowser.Url = new Uri(htmlLocation);
            }
            else
            {
                throw new Exception("HTML file not found!");
            }
        }

        /// <summary>
        /// Initializes Virtual Earth
        /// </summary>
        private void vEarthBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!this.DesignMode)
            {
                vEarthBrowser.Document.InvokeScript("startVE");

                Console.WriteLine("vEarthBrowser_DocumentCompleted");
            }
        }

        /// <summary>
        /// Resizes the map
        /// </summary>
        private void vEarthBrowser_Resize(object sender, EventArgs e)
        {
            if (!this.DesignMode && vEarthBrowser.Document != null && vEarthBrowser.Document.GetElementById("myMap") != null)
            {
                vEarthBrowser.Document.GetElementById("myMap").Style = "overflow: hidden; width: 100%; height: 100%;";
                vEarthBrowser.Document.InvokeScript("maxSize");
            }
        }

        #region Properties
        /// <summary>
        /// Flag indicating that a call to VE_FindLocations is finished
        /// </summary>
        public bool FinishedFindingLocations
        {
            get
            {
                return finishedFindingLocations;
            }
        }

        private string htmlLocation = "VirtualEarth.html";

        [Browsable(true), 
        DefaultValue("VirtualEarth.html"), 
        Description("The location of the html file with the code for accessing Microsoft Live Maps."), 
        Category("Virtual Earth"),
        EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor),typeof(System.Drawing.Design.UITypeEditor))] 
        public string HTMLLocation
        {
            get { return htmlLocation; }
            set 
            { 
                htmlLocation = value;

                if (!this.DesignMode)
                {
                    vEarthBrowser.ObjectForScripting = this;

                    if (System.IO.File.Exists(htmlLocation))
                    {
                        vEarthBrowser.Url = new Uri(htmlLocation);
                        vEarthBrowser.Document.InvokeScript("startVE");
                    }
                }
            }
        }

        private DistanceUnitEnum distanceUnit = DistanceUnitEnum.Kilometers;

        [Browsable(true), 
        Description("The distance unit used."),
        DefaultValue(ucVEarth.DistanceUnitEnum.Kilometers),
        Category("Virtual Earth")]
        public DistanceUnitEnum DistanceUnit
        {
            get { return distanceUnit; }
            set 
            { 
                distanceUnit = value;

                if (!this.DesignMode && vEarthBrowser.Document != null)
                {
                    vEarthBrowser.Document.InvokeScript("setDistanceUnit", new object[] { (int)distanceUnit });
                }
            }
        }

        private DisambiguationEnum disambiguationMode = DisambiguationEnum.Default;

        [Browsable(true),
        Description("The disambiguation mode."),
        DefaultValue(ucVEarth.DisambiguationEnum.Default),
        Category("Virtual Earth")]
        public DisambiguationEnum DisambiguationMode
        {
            get { return disambiguationMode; }
            set
            {
                disambiguationMode = value;

                if (!this.DesignMode && vEarthBrowser.Document != null)
                {
                    vEarthBrowser.Document.InvokeScript("setDisambiguationMode", new object[] { (int)disambiguationMode });
                }
            }
        }

        private DashboardStyleEnum dashboardStyle = DashboardStyleEnum.Normal;

        [Browsable(true),
        Description("The size of the dashboard."),
        DefaultValue(ucVEarth.DashboardStyleEnum.Normal),
        Category("Virtual Earth")]
        public DashboardStyleEnum DashboardStyle
        {
            get { return dashboardStyle; }
            set
            {
                dashboardStyle = value;

                if (!this.DesignMode && vEarthBrowser.Document != null)
                {
                    vEarthBrowser.Document.InvokeScript("setDashboardStyle", new object[] { (int)dashboardStyle });
                }
            }
        }

        private MapStyleEnum mapStyle = MapStyleEnum.Road;

        [Browsable(true),
        Description("Defines the way the map is shown."),
        DefaultValue(ucVEarth.MapStyleEnum.Road),
        Category("Virtual Earth")]
        public MapStyleEnum MapStyle
        {
            get { return mapStyle; }
            set
            {
                mapStyle = value;

                if (!this.DesignMode && vEarthBrowser.Document != null)
                {
                    vEarthBrowser.Document.InvokeScript("setMapStyle", new object[] { (int)mapStyle });
                }
            }
        }

        private bool showDashboard = true;

        [Browsable(true),
        Description("Show or hide the dashboard."),
        DefaultValue(true),
        Category("Virtual Earth")]
        public bool ShowDashboard
        {
            get { return showDashboard; }
            set
            {
                showDashboard = value;

                if (!this.DesignMode && vEarthBrowser.Document != null)
                {
                    vEarthBrowser.Document.InvokeScript("setDashboardVisibility", new object[] { showDashboard });
                }
            }
        }

        private string mapLocation = ""; 

        [Browsable(true),
        Description("Set the location displayed on the map."),
        DefaultValue(""), 
        Category("Virtual Earth")]
        public string MapLocation
        {
            get { return mapLocation; }
            set
            {
                mapLocation = value;

                if (!this.DesignMode && vEarthBrowser.Document != null)
                {
                    vEarthBrowser.Document.InvokeScript("findLocation", new object[] { mapLocation });
                }
            }
        }

        private int zoomLevel = 10;

        [Browsable(true),
        Description("Set the zoomlevel. (1 to 19)"),
        DefaultValue(10),
        Category("Virtual Earth")]
        public int ZoomLevel
        {
            get 
            {
                if (!this.DesignMode && vEarthBrowser.Document != null)
                {
                    return (int)vEarthBrowser.Document.InvokeScript("getZoomLevel");
                }
                else
                {
                    return zoomLevel;
                }
            }
            set
            {
                zoomLevel = value;

                if (!this.DesignMode && vEarthBrowser.Document != null)
                {
                    vEarthBrowser.Document.InvokeScript("find", new object[] { zoomLevel });
                }
            }
        }

        public List<SearchLocation> SearchLocations
        {
            get { return locations; }
        }
	
	    #endregion

        #region Virtual Earth map control methods
        /// <summary>
        /// Set zoom level of the map
        /// </summary>
        /// <param name="level">The level to zoom to (1 to 19)</param>
        public void VE_SetZoomLevel(int level)
        {
            if (level < 1 || level > 19)
            {
                throw new Exception("Level must be a number between 1 and 19."); 
            }
            
            if (!this.DesignMode && vEarthBrowser.Document != null)
            {
                vEarthBrowser.Document.InvokeScript("setZoomLevel", new object[] { level });
            }
        }

        /// <summary>
        /// Zoom in 1 level on the map
        /// </summary>
        public void VE_ZoomIn()
        {
            if (!this.DesignMode && vEarthBrowser.Document != null)
            {
                vEarthBrowser.Document.InvokeScript("zoomIn");
            }
        }

        /// <summary>
        /// Zoom out 1 level on the map
        /// </summary>
        public void VE_ZoomOut()
        {
            if (!this.DesignMode && vEarthBrowser.Document != null)
            {
                vEarthBrowser.Document.InvokeScript("zoomOut");
            }
        }

        /// <summary>
        /// Add a shape layer to the map
        /// </summary>
        /// <param name="title">Title of the layers</param>
        /// <param name="description">Description of the layer</param>
        /// <remarks>Shapelayers are identified by their title</remarks> 
        public void VE_AddShapeLayer(string title, string description)
        {
            if (!this.DesignMode && vEarthBrowser.Document != null)
            {
                vEarthBrowser.Document.InvokeScript("addShapeLayer", new object[] { title, description });
            }
        }

        /// <summary>
        /// Remove as shapelayer and its contents from the map
        /// </summary>
        /// <param name="title">The title of the layer to remove</param>
        /// <returns>false if the layer cannot be found</returns>
        /// <remarks>Shapelayers are identified by their title</remarks> 
        public bool VE_DeleteShapeLayer(string title)
        {
            if (!this.DesignMode && vEarthBrowser.Document != null)
            {
                return (bool)vEarthBrowser.Document.InvokeScript("deleteShapeLayer", new object[] { title });
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Delete all shapelayers and their content from the map
        /// </summary>
        public void VE_DeleteAllShapeLayers()
        {
            vEarthBrowser.Document.InvokeScript("deleteAllShapeLayers");
        }

        /// <summary>
        /// Set the visibility of a shapelayer
        /// </summary>
        /// <param name="title">The title of the shapelayer to modify</param>
        /// <param name="visible">true to show the shapelayer, false to hide</param>
        /// <returns>false if the layer cannot be found</returns>
        /// <remarks>Shapelayers are identified by their title</remarks> 
        public bool VE_SetShapeLayerVisible(string title, bool visible)
        {
            return (bool)vEarthBrowser.Document.InvokeScript("setShapeLayerVisible", new object[] { title, visible });
        }

        /// <summary>
        /// Search for multiple locations and display their location on the map using a pushpin.
        /// </summary>
        /// <param name="locations">List of locations to search for</param>
        /// <param name="centerAtLastResult">if true, the map will be centered at the last found location</param> 
        /// <param name="displayResults">if true, results will be shown on the map as pushpins</param> 
        /// <remarks>
        /// Use this function to find locations by where or what. If the latitude and Longitude is known, use
        /// the AddPushPin function instead.
        /// After a succesful call the latitude and Longitude of the locations in the list is set
        /// If a location is not found, the latitude and Longitude will be null
        /// </remarks> 
        public void VE_FindLocations(List<SearchLocation> locations, bool centerAtLastResult, bool displayResults, MethodInvoker callbackFunction)
        {
            this.locations = locations;
            this.centerAtLastResult = centerAtLastResult;
            this.displayResults = displayResults;

            if(locations.Count > 0)
            {
                finishedFindingLocations = false;

                searchLocationIndex = 0;
                SearchLocation searchLocation = locations[searchLocationIndex];
                vEarthBrowser.Document.InvokeScript("find", new object[] { searchLocation.What, searchLocation.Where, centerAtLastResult });
            }

            if (callbackFunction != null)
            {
                VE_OnFinishedFindingLocations += new FinishedFindingLocations(callbackFunction);
            }
        }

        /// <summary>
        /// Callback function for the find call to the Virtual Earth control, adds the latitude and Longitude to the locations
        /// </summary>
        /// <param name="Lat">Latitude of the found location, or null when the location is not found</param>
        /// <param name="Long">Longitude of the found location, or null when the location is not found</param>
        /// <param name="errorMsg">Error message if an error has occured</param>
        /// <remarks>This method should not be called directly, instead use the VE_DisplayLocations(..) method</remarks> 
        public void OnJavascriptLocationFound(object Lat, object Long, object errorMsg)
        {
            locations[searchLocationIndex].Latitude = (double?)Lat;
            locations[searchLocationIndex].Longitude = (double?)Long;

            if (locations != null && locations.Count > searchLocationIndex + 1)
            {
                SearchLocation searchLocation = locations[++searchLocationIndex];
                vEarthBrowser.Document.InvokeScript("find", new object[] { searchLocation.What, searchLocation.Where });
            }
            else if(locations != null && displayResults)
            {   // all locations are search for, now display the results if specified
                DisplayLocations();
                finishedFindingLocations = true;
            }
            else if (centerAtLastResult && locations.Count > 0 && !displayResults)
            {   // if the results will not be displayed but the map should be centered at the last location, then do that now
                VE_SetCenter(locations[locations.Count - 1]);
                finishedFindingLocations = true;
            }
            else
            {
                finishedFindingLocations = true;
            }

            // Trigger public event
            if (finishedFindingLocations)
            {
                VE_OnFinishedFindingLocations();
            }
        }

        /// <summary>
        /// Add pushpins for all locations in the collection
        /// </summary>
        private void DisplayLocations()
        {
            foreach (SearchLocation location in locations)
            {
                if (location.Latitude != null && location.Longitude != null)
                {
                    object id = vEarthBrowser.Document.InvokeScript("addPushPin", 
                        new object[] { 
                            location.Latitude, 
                            location.Longitude, 
                            location.PushPinTitle, 
                            location.PushPinLayer, 
                            location.PushPinImage, 
                            location.PushPinDescription });

                    location.ID = id == null ? "" : id.ToString();
                }
            }

           // if specified, the last result will be centered on the map
            if (centerAtLastResult && locations.Count > 0)
            {
                VE_SetCenter(locations[locations.Count - 1]);
            }
        }

        /// <summary>
        /// Centers the map at the specified location (based on latitude/Longitude or where/what)
        /// </summary>
        /// <param name="location">The location the map will center at</param>
        public void VE_SetCenter(SearchLocation location)
        {
            // set center by coordinates...
            if (location.Latitude != null && location.Longitude != null)
            {
                vEarthBrowser.Document.InvokeScript("setCenter",
                        new object[] 
                        { 
                            location.Longitude, 
                            location.Latitude 
                        });
            } // ...or by what / where
            else if (location.Where != String.Empty || location.What != String.Empty)
            {
                locations = new List<SearchLocation>();
                SearchLocations.Add(location);
                VE_FindLocations(locations, true, false, null);    
            }
        }

        /// <summary>
        /// Initiate route calculations
        /// </summary>
        /// <param name="locations">The waypoints of the route (max 25)</param>
        /// <param name="displayRoute">Display the route on the map yes or no</param>
        /// <param name="setBestMapView">Center the map to have best view of the route yes or no</param>
        /// <param name="routeMode">Walking or driving directions</param>
        /// <param name="distanceUnit">Miles or kilometers</param>
        /// <param name="routeOptimizeFactor">Optimize for minimal time or minimal distance</param> 
        /// <returns>RouteDirection object </returns> 
        /// <remarks>If the routeOptimizeFactor is set to Time and the RouteMode is set to walking, then an empty object is returned.</remarks> 
        public RouteDirections VE_GetDirections(List<SearchLocation> locations, bool displayRoute, bool setBestMapView, RouteModeEnum routeMode, DistanceUnitEnum distanceUnit, RouteOptimizeEnum routeOptimizeFactor)
        {
            List<SearchLocation> locationsWithLatLongCoordinates = new List<SearchLocation>();
            List<SearchLocation> locationsWithoutLatLongCoordinates = new List<SearchLocation>();
  
            if (locations.Count > 25)
            {
                throw new Exception("Maximum number of waypoints in a route is 25."); 
            }

            // split locations in those with and without latitude and longtidude information
            foreach (SearchLocation location in locations)
            {
                if (location.Latitude != null && location.Longitude != null)
                {
                    locationsWithLatLongCoordinates.Add(location);
                }
                else
                {
                    locationsWithoutLatLongCoordinates.Add(location);  
                }
            }

            // if there are locations without latitude and Longitude information, find the lat/long coordinates using where or what
            if (locationsWithoutLatLongCoordinates.Count > 0)
            {
                VE_FindLocations(locationsWithoutLatLongCoordinates, false, false, null);

                while (!finishedFindingLocations)
                {
                    Application.DoEvents();
                }

                locationsWithLatLongCoordinates.AddRange(this.locations);   
            }

            vEarthBrowser.Document.InvokeScript("clearRouteWaypoints");

            // add the waypoints by latitude and Longitude
            foreach (SearchLocation searchLocation in locationsWithLatLongCoordinates)
            {
                if (searchLocation.Latitude != null && searchLocation.Longitude != null)
                {
                    vEarthBrowser.Document.InvokeScript("addRouteWaypoint",
                        new object[] 
                        { 
                            searchLocation.Longitude, 
                            searchLocation.Latitude 
                        });
                }
            }

            routeDirections = null;

            object waypoints = vEarthBrowser.Document.InvokeScript("getDirections", new object[] { displayRoute, setBestMapView, (int)routeMode, distanceUnit, (int)routeOptimizeFactor });

            // wait for asynchronous call to getDirections callback function to finish
            while (routeDirections == null)
            {
                Application.DoEvents();  
            }

            return routeDirections;
        }

        /// <summary>
        /// Callback function for VE_GetDirections
        /// </summary>
        /// <param name="time">Total time in seconds</param>
        /// <param name="distance">Total distance in miles or kilometers</param>
        /// <param name="description">Route description</param>
        public void OnDirectionsFinished(object duration, object distance, object description)
        {
            System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
                  
            routeDirections = new RouteDirections(double.Parse(duration.ToString()), decimal.Parse(distance.ToString(), nfi));

            foreach (string routeLeg in ((string)description).Trim('|').Split('|'))
            {
                string[] routeLegDetails = routeLeg.Split('~');  
                routeDirections.AddRouteLeg(new RouteLeg(routeLegDetails[0], double.Parse(routeLegDetails[2]), decimal.Parse(routeLegDetails[1], nfi)));   
            }
        }

        /// <summary>
        /// Add a pushpin to the map based on Latitude and Longitude coordinates
        /// </summary>
        /// <param name="location">The location and appearance of the pushpin</param>
        /// <returns>The unique identifier of the added puspin (empty when to location could not be found)</returns>
        public string VE_AddPushPin(SearchLocation location)
        {
            if (location.Latitude == null && location.Longitude == null)
            {
                throw new Exception("Specify the Latitude and Longitude of the pushpin.");
            }

            object id = vEarthBrowser.Document.InvokeScript("addPushPin",
                        new object[] { 
                            location.Latitude, 
                            location.Longitude, 
                            location.PushPinTitle, 
                            location.PushPinLayer, 
                            location.PushPinImage, 
                            location.PushPinDescription });

            location.ID = id == null ? "" : id.ToString();

            return location.ID;
        }

        /// <summary>
        /// Add a line to the map between the given points
        /// </summary>
        /// <param name="locations">A list of points that make up the line</param>
        /// <returns>The unique identifier of the added line</returns>
        /// <remarks>Location that cannot be resolved to Lat/Long coordinates will be excluded</remarks> 
        public string VE_AddShape(VE_Shape shape, List<SearchLocation> pointsOfShape, string title, string description, string layer, int linewidth)
        {
            List<SearchLocation> locationsWithLatLongCoordinates = new List<SearchLocation>();
            List<SearchLocation> locationsWithoutLatLongCoordinates = new List<SearchLocation>();

            // split locations in those with and without latitude and longtidude information
            foreach (SearchLocation location in pointsOfShape)
            {
                if (location.Latitude != null && location.Longitude != null)
                {
                    locationsWithLatLongCoordinates.Add(location);
                }
                else
                {
                    locationsWithoutLatLongCoordinates.Add(location);
                }
            }

            // if there are locations without latitude and Longitude information, find the lat/long coordinates using where or what
            if (locationsWithoutLatLongCoordinates.Count > 0)
            {
                VE_FindLocations(locationsWithoutLatLongCoordinates, false, false, null);

                while (!finishedFindingLocations)
                {
                    Application.DoEvents();
                }

                locationsWithLatLongCoordinates.AddRange(this.locations);
            }

            vEarthBrowser.Document.InvokeScript("clearRouteWaypoints");

            // add the points by latitude and Longitude
            foreach (SearchLocation searchLocation in locationsWithLatLongCoordinates)
            {
                if (searchLocation.Latitude != null && searchLocation.Longitude != null)
                {
                    vEarthBrowser.Document.InvokeScript("addRouteWaypoint",
                        new object[] 
                        { 
                            searchLocation.Longitude, 
                            searchLocation.Latitude 
                        });
                }
            }

            object id = vEarthBrowser.Document.InvokeScript(shape == VE_Shape.Polyline ? "addPolyline" : "addPolygon", 
                new object[] 
                {  
                    title,
                    description,
                    layer,
                    linewidth
                });

            return id == null ? "" : id.ToString();
        }

        /// <summary>
        /// Removes a pushpin from the map
        /// </summary>
        /// <param name="id">The unique identifier of the pushpin to remove</param>
        public void VE_DeletePushPin(string id)
        {
            vEarthBrowser.Document.InvokeScript("deletePushPin", new object[] { id });  
        }

        /// <summary>
        /// Delete all pushpins, lines and polygones from the map
        /// </summary>
        public void VE_DeleteAllShapes()
        {
            vEarthBrowser.Document.InvokeScript("deleteAllShapes");  
        }

        /// <summary>
        /// Delete current route directions from the map
        /// </summary>
        public void VE_DeleteDirections()
        {
            vEarthBrowser.Document.InvokeScript("deleteDirections");
        }

        /// <summary>
        /// Save the exact appearance of the map as an image
        /// </summary>
        /// <param name="format">Image format (jpg, gif, png etc)</param>
        /// <param name="path">Filename of the image. Existing files will be overwritten.</param>
        public void VE_SaveMapImage(ImageFormat format, string path)
        {
            FormPrint.Save(this, format, path);  
        }
        #endregion
    }
}