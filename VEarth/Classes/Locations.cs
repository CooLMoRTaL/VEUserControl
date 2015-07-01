using System;
using System.Collections.Generic;
using System.Text;

namespace VEarth.Locations
{
    #region SearchLocation class
    /// <summary>
    /// This calls represents a location on the map. 
    /// </summary>
    public class SearchLocation
    {
        public SearchLocation()
        {
        }

        /// <summary>
        /// The search string for the location
        /// </summary>
        public string Where = "";

        /// <summary>
        /// The search string for the business
        /// </summary>
        public string What = "";

        /// <summary>
        /// Title displayed on the pushpin
        /// </summary>
        public string PushPinTitle = "";

        /// <summary>
        /// Description displayed when the pushpin is selected
        /// </summary>
        public string PushPinDescription = "";

        /// <summary>
        /// Location of the image used as pushpin symbol on the map
        /// </summary>
        public string PushPinImage = "";

        /// <summary>
        /// Title of the layer to add the pushpin to, leave empty to not add the pushpin to a layer
        /// </summary>
        public string PushPinLayer = "";

        /// <summary>
        /// Longitude of the found location, do not set it by code: it gets filled during the search operation
        /// </summary>
        public double? Longitude = null;

        /// <summary>
        /// Latitude of the found location, do not set it by code: it gets filled during the search operation
        /// </summary>
        public double? Latitude = null;

        /// <summary>
        /// ID of the pushpin once it is displayed on the map. Use this ID to delete or modify a pushpin
        /// </summary>
        public string ID = "";
    }
    #endregion
}
