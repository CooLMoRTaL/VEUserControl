using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using VEarth;
using VEarth.Directions;
using VEarth.Locations;   

namespace Container
{
    public partial class FrmDemo : Form
    {
        public FrmDemo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Init the map control
        /// </summary>
        private void FrmDemo_Shown(object sender, EventArgs e)
        {
            ucVEarth.InitMap();    // Initialize the VEarth user control
            ucVEarth.VE_OnMouseClick += new MouseClick(ucVEarth_VE_OnMouseClick);
            ucVEarth.VE_OnFinishedFindingLocations += new FinishedFindingLocations(ucVEarth_VE_OnFinishedFindingLocations); 
        }

        void ucVEarth_VE_OnMouseClick(double latitude, double longitude, object id, bool altKey, bool ctrlKey, bool shiftKey, bool leftButton, bool rightButton)
        {
            SearchLocation loc = new SearchLocation();
            loc.Latitude = latitude;
            loc.Longitude = longitude;
            loc.PushPinDescription = String.Format("Lat: {0}, Long: {1}", latitude, longitude);
            loc.PushPinTitle = "You've put me on the map";
            ucVEarth.VE_AddPushPin(loc); 
        }

        /// <summary>
        /// Display pushpins for all adresses in the textbox
        /// </summary>
        private void btnDisplayAddresses_Click(object sender, EventArgs e)
        {
            // For demo pusposes, add the pushpins to a layer named "Test"
            ucVEarth.VE_AddShapeLayer("Test", "Test");

            // Create a list of locations to search for
            List<SearchLocation> locations = new List<SearchLocation>();
            foreach (string line in txtAddresses.Lines)
            {
                SearchLocation a = new SearchLocation();
                a.Where = line;  // where to search for (address)
                a.PushPinDescription = line;
                a.PushPinLayer = "Test";
                a.PushPinTitle = line;  // Title of the puspin
                locations.Add(a);
            }

            // find and display the locations, during the seacht the latitude and Longitude of each SearchLocation object is 
            // set, so VEarth knows where to place the pushpins
            ucVEarth.VE_FindLocations(locations, true, true, null);
        }

        void ucVEarth_VE_OnFinishedFindingLocations()
        {
            // at this point, after calling VE_FindLocations, each SearchLocation in ucVEarth.SearchLocations has
            // the properties Latitude and Longitude set. You can store these in for example a database so the next
            // time showing pushpins is faster because Virtual Earth does not have to translate the address to Latitude
            // and Longitude coordinates
            ucVEarth.VE_SaveMapImage(System.Drawing.Imaging.ImageFormat.Jpeg, @"c:\testing.jpg");
            Console.WriteLine("ucVEarth_VE_OnFinishedFindingLocations");  
        }

        /// <summary>
        /// Save the entered locations
        /// </summary>
        private void FrmDemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            global::Container.Properties.Settings.Default.Save();     
        }

        /// <summary>
        /// Hide the layer with the pushpins
        /// </summary>
        private void btnHideLayer_Click(object sender, EventArgs e)
        {
            if (!ucVEarth.VE_SetShapeLayerVisible("Test", false))
            {
                MessageBox.Show("Layer does not exists");
            } 
        }

        /// <summary>
        /// Show the layer with the pushpins
        /// </summary>
        private void btnShowLayer_Click(object sender, EventArgs e)
        {
            if(!ucVEarth.VE_SetShapeLayerVisible("Test", true))
            {
                MessageBox.Show("Layer does not exists");  
            }
        }

        /// <summary>
        /// Get the driving directions for the adresses in the listbox (the API has a maximum of 50 addressen)
        /// </summary>
        private void btnGetDirections_Click(object sender, EventArgs e)
        {
            ucVEarth.VE_DeleteDirections(); 
            
            // Add the start and end point
            List<SearchLocation> locations = new List<SearchLocation>();
            foreach (string line in txtAddresses.Lines)
            {
                SearchLocation a = new SearchLocation();
                a.Where = line;  // where to search for (address)
                locations.Add(a);
            }
            //Clear current directions
            lvDirections.Items.Clear();

            // Get and display driving directions
            RouteDirections rd = ucVEarth.VE_GetDirections(locations, true, true, VEarth.ucVEarth.RouteModeEnum.Driving, VEarth.ucVEarth.DistanceUnitEnum.Kilometers, ucVEarth.RouteOptimizeEnum.Distance );

            lvDirections.Items.Add(String.Format("Total distance: {0}", rd.TotalDistance));
            lvDirections.Items.Add(String.Format("Total duration: {0}", GetDurationAsText(rd.TotalDuration)));
            lvDirections.Items.Add("");

            decimal totalDistance = 0;
            double totalDuration = 0;

            // Iterate through all route parts
            foreach(RouteLeg rl in rd.RouteLegs)
            {
                totalDistance += rl.Distance;
                totalDuration += rl.Duration;

                ListViewItem direction = new ListViewItem(rl.Description);   
                direction.SubItems.Add(rl.Distance.ToString());
                direction.SubItems.Add(GetDurationAsText(rl.Duration));
                direction.SubItems.Add(totalDistance.ToString());
                direction.SubItems.Add(GetDurationAsText(totalDuration));  
                lvDirections.Items.Add(direction);

                
            }
        }

        /// <summary>
        /// Gewt duration (in seconds) as formatted text
        /// </summary>
        /// <param name="duration">duration in seconds</param>
        /// <returns>String with formatted time in the format HH:mm:ss</returns>
        private string GetDurationAsText(double duration)
        {
            TimeSpan t = TimeSpan.FromSeconds(duration);
            return String.Format("{0}:{1}:{2}", t.Hours.ToString().PadLeft(2, '0'),
                                                t.Minutes.ToString().PadLeft(2, '0'),
                                                t.Seconds.ToString().PadLeft(2, '0'));
        }

        private void FrmDemo_Load(object sender, EventArgs e)
        {

        }

        private void btnDisplayLine_Click(object sender, EventArgs e)
        {
            List<SearchLocation> locations = new List<SearchLocation>();
            foreach (string line in txtAddresses.Lines)
            {
                SearchLocation a = new SearchLocation();
                a.Where = line;  // where to search for (address)
                locations.Add(a);
            }

            ucVEarth.VE_AddShape(VE_Shape.Polyline, locations, "My Path", "A sample path", "", 10); 
        }

        private void btnDisplayPolygon_Click(object sender, EventArgs e)
        {
            List<SearchLocation> locations = new List<SearchLocation>();

            if (txtAddresses.Lines.Length < 3)
            {
                MessageBox.Show("To create a polygon a minimum of 3 points is needed");  
            }
            else
            {
                foreach (string line in txtAddresses.Lines)
                {
                    SearchLocation a = new SearchLocation();
                    a.Where = line;  // where to search for (address)
                    locations.Add(a);
                }

                ucVEarth.VE_AddShape(VE_Shape.Polygon, locations, "My Polygon", "A sample polygon", "", 10);
            }
        }
    }
}