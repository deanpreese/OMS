using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algo.Algorithms.Utility
{
    public static class EngineUtilities
    {


/*
        // ----------------------------------------------------------
        public static int GetBaseLeverage(string trader, bool UseOverride, int OverrideLeverage, int[] GroupsArray)
        {
            int leverage = 0;

            UserProfileHelper ph = new UserProfileHelper();
            Platform.Shared.UserProfileData up = ph.GetUserProfile(trader);
            int defaultUserLeverage = Convert.ToInt32(up.Leverage);
            

            if (GroupsArray.Contains(up.GroupNumber))
            {
                if (up.OppositeTrader > 0)
                {
                    leverage = defaultUserLeverage;

                    if (UseOverride)
                    {
                        leverage = OverrideLeverage;
                    }
                }
            }

            //Console.WriteLine(trader + "  Base Leverage: " + leverage);
            return leverage;
        }

*/
        // ----------------------------------------------------------
        public static int ConvertOrderAction(int oa)
        {
            int action = 0;

            // ===============
            // Buying to Cover  2   Selling -1
            // Buying 1    Selling Short   -2

            //Reverse the buy orders
            if (oa == 1 || oa == 2)
            {
                action = -1;
            }

            // Reverse the sell orders
            if (oa == -1 || oa == -2)
            {
                action = 1;
            }

            return action;
        }



        // ----------------------------------------------------------
        public static string OrderActionToString( int oa)
        {
            string rtn = "";

            //Reverse the buy orders
            if (oa == 1 || oa == 2)
            {
                rtn = "Buy";
            }

            // Reverse the sell orders
            if (oa == -1 || oa == -2)
            {
                rtn = "Sell";
            }


            return rtn;
        }






    }
}
