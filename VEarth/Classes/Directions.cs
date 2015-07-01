using System;
using System.Collections.Generic;
using System.Text;

namespace VEarth.Directions
{
    #region RouteLeg class
    public class RouteLeg
    {
        #region Properties
        private double duration = 0;

        /// <summary>
        /// Time between two waypoints in a route  (format is hh:mm:ss)
        /// </summary>
        public double Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        private decimal distance = 0;

        /// <summary>
        /// Distance in miles or kilometers of this part of the route
        /// </summary>
        public decimal Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        private string description = String.Empty;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        #endregion

        #region Constructors
        public RouteLeg()
        {
        }

        public RouteLeg(string description, double duration, decimal distance)
        {
            this.description = description;
            this.duration = duration;
            this.distance = distance;
        }
        #endregion
    }
    #endregion

    #region RouteDirections class
    public class RouteDirections
    {
        #region Constructors
        public RouteDirections()
        {
        }

        public RouteDirections(double totalDuration, decimal totalDistance)
        {
            this.totalDuration = totalDuration;
            this.totalDistance = totalDistance;
        }
        #endregion

        #region Properties
        private List<RouteLeg> routeLegs = new List<RouteLeg>();

        public List<RouteLeg> RouteLegs
        {
            get { return routeLegs; }
            set { routeLegs = value; }
        }

        private double totalDuration = 0;

        /// <summary>
        /// Total time of a route (format is hh:mm:ss)
        /// </summary>
        public double TotalDuration
        {
            get { return totalDuration; }
            set { totalDuration = value; }
        }

        private decimal totalDistance = 0;

        // Total distance (in miles or kilometers)
        public decimal TotalDistance
        {
            get { return totalDistance; }
            set { totalDistance = value; }
        }
        #endregion

        /// <summary>
        /// Adds a part of the route to the directions 
        /// </summary>
        /// <param name="routeLeg">The leg to add</param>
        public void AddRouteLeg(RouteLeg routeLeg)
        {
            routeLegs.Add(routeLeg);
        }
    }
    #endregion
}
