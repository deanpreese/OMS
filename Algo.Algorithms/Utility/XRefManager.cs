using System.Text;

using Algo.Algorithms.Models;


namespace Algo.Algorithms.Utility
{
    public class XRefManager
    {

        private ColoredScreenWriter csw = new ColoredScreenWriter(0);

        // ----------------------------------------------------------
        public void ResetXRefsByInstrument( int qty, int rtnLOVal, string instrument, string algoTrader, string msg)
        {
            try
            {
                /*
                    var entities = new AlgoXRef4Entities();
                    List<TraderXRef> traderData = (from t in entities.TraderXRefs
                                                   where t.Instrument == instrument
                                                             && t.AlgoTrader == algoTrader
                                                   select t).ToList();

                    csw.LogYellowType(1, "Trades to be closed: " + traderData.Count() + "  for " + instrument);


                    if (traderData.Any())
                    {
                        foreach (TraderXRef t in traderData)
                        {
                            t.LevQuantity = 0;
                            entities.SaveChanges();
                        }
                    }
                */
            }
            catch (Exception e)
            {
                
                csw.LogRedType(10000, "Error: ResetXRefsByInstrument -- " +  e.ToString());
            }
            
           
        }


        // ----------------------------------------------------------
        public void UpdateAllCrossRef(int qty, int rtnLOVal, string trader, string instrument, string algoTrader,string msg)
        {
            try
            {
                UpdateLocalLeverageQuantity(qty, trader, instrument, algoTrader);
                UpdateLocalCrossReferenceActuals(trader, instrument, algoTrader);
                CheckLocalLeverageVersusActual(rtnLOVal, trader, instrument, algoTrader, msg);
                
                /*
                UpdateServerXRef(algoTrader, trader, instrument);
                */


            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: UpdateAllCrossRef -- " + e.ToString());
            }
        }


        // ----------------------------------------------------------
        // Manage Local Actuals
        // ----------------------------------------------------------
        public int UpdateLocalCrossReferenceActuals(string trader, string instrument, string algoTrader)
        {
            int total = 0;

            try
            {
                /*
                var loh = new LiveOrderHelper();
                List<OrderData> openOrderList = loh.GetOrdersByTrader(trader);

                if (openOrderList.Count() > 0)
                {
                    IEnumerable<OrderData> matchingOrders = from o in openOrderList
                                                            where o.Instrument == instrument
                                                            select o;

                    if (matchingOrders.Any())
                    {
                        // set the Actual Qty to the total value
                        total = matchingOrders.Sum(x => x.Quantity);

                        // Buying to Cover  2   Selling -1
                        // Buying 1    Selling Short   -2


                        // return a negative number signifying you are short
                        // return a positive number signifying you are long

                        if (matchingOrders.First().OrderAction == -1 || matchingOrders.First().OrderAction == -2)
                        {
                            total = total * -1;
                        }

                        csw.Log(1, "Updating LOCAL CrossRef QTY: " + total + "   Direction: " + EngineUtilities.OrderActionToString(matchingOrders.First().OrderAction));
                        UpdateLocalActualQuantity(total, trader, instrument, algoTrader);
                    }

                    
                }
                else
                {
                    // no orders exist
                    // reset actual to zero
                    UpdateLocalActualQuantity(0, trader, instrument, algoTrader);
                }
                */
            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: UpdateLocalCrossReferenceActuals -- " + e.ToString());
            }

           
            return total;
        }


        // ----------------------------------------------------------
        public int GetLocalActualQuantity(string trader, string instrument, string algoTrader)
        {
            int rtnVal = 0;

            /*
            try
            {
                var entities = new AlgoXRef4Entities();

                List<TraderXRef> traderData = (from t in entities.TraderXRefs
                                               where t.TraderID == trader
                                                     && t.Instrument == instrument
                                                         && t.AlgoTrader == algoTrader
                                               select t).ToList();
                if (traderData.Count() > 0)
                {
                    rtnVal = (int)traderData.First().ActualQuantity;
                }
            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: GetLocalActualQuantity -- " +  e.ToString());
            }
            */
            return rtnVal;
        }

        // ----------------------------------------------------------
        private void UpdateLocalActualQuantity(int qty, string trader, string instrument, string algoTrader)
        {
            try
            {
                /*        
                    var entities = new AlgoXRef4Entities();

                    List<TraderXRef> traderData = (from t in entities.TraderXRefs
                                                   where t.TraderID == trader
                                                         && t.Instrument == instrument
                                                          && t.AlgoTrader == algoTrader
                                                   select t).ToList();

                    if (traderData.Count() > 0)
                    {
                        // row exists
                        foreach (TraderXRef traderXref in traderData)
                        {
                            try
                            {
                                traderXref.ActualQuantity = qty;
                                entities.SaveChanges();
                                csw.Log(1, "Update Local Actual Quantity for: " + instrument + "  " + trader + "  Quantity: " + qty);
                            }
                            catch (Exception eSave)
                            {
                                csw.LogRed(1, " --------------------------------------------------- ");
                                csw.LogRed(1, "Error Saving Record ");
                                csw.LogRed(1, eSave.ToString());
                            }
                        }
                    }
                    else
                    {
                        try
                        {


                            // create new row

                            TraderXRef tt = new TraderXRef();
                            tt.TraderID = trader;
                            tt.AlgoTrader = algoTrader;
                            tt.Instrument = instrument;
                            tt.ActualQuantity = qty;
                            tt.LevQuantity = 0;
                            tt.RID = DateTime.UtcNow.Ticks;

                            
                            TraderXRef t = TraderXRef.CreateTraderXRef(
                                trader, qty, 0, instrument, DateTime.UtcNow.Ticks);
                            t.AlgoTrader = algoTrader;
                            
                            entities.TraderXRefs.Add(tt);
                            entities.SaveChanges();
                            csw.Log(1, "Update Local Actual Quantity - Adding Record for: " + instrument + "  " + trader + "  Quantity: " +  qty);
                        }
                        catch (Exception eAdd)
                        {
                            csw.LogRed(1, " --------------------------------------------------- ");
                            csw.LogRed(1, "Error Adding Record ");
                            csw.LogRed(1, eAdd.ToString());
                            csw.LogRed(1, " --------------------------------------------------- ");
                        }
                    }

                */


            }
            catch (Exception eeee)
            {
                 csw.LogRed(1,"Error - Update Local Actual Quantity: " +  eeee.ToString());
            }
        }


        // ----------------------------------------------------------
        // Manager Local Leverage 
        // ----------------------------------------------------------
        private void UpdateLocalLeverageQuantity(int qty, string trader, string instrument, string algoTrader)
        {
            try
            {
                    /*
                    
                    var entities = new AlgoXRef4Entities();
                    List<TraderXRef> traderData = (from t in entities.TraderXRefs
                                                   where t.TraderID == trader
                                                         && t.Instrument == instrument
                                                               && t.AlgoTrader == algoTrader
                                                   select t).ToList();

                    csw.LogCyanType(1, "Updaing Local Leverage Quantity for: " + instrument + "  " + trader + "  Quantity: " + qty  + "  ALGO:  "  +  algoTrader );


                    if (traderData.Any())
                    {
                        foreach (TraderXRef traderXRef in traderData)
                        {

                            try
                            {
                                traderXRef.LevQuantity = qty;
                                entities.SaveChanges();
                                csw.LogGreenType(1, "UPDATED  -- " + qty);
                            }
                            catch (Exception eUpdate)
                            {
                                csw.LogRedType(10000, eUpdate.StackTrace  );
                            }

                            
                        }    
                    }
                    else
                    {
                        csw.LogRedType(1, " ---------------------- ");
                        csw.LogRedType(1, "None to UPDATE");
                        csw.LogRedType(1, " ---------------------- ");
                    }
                */

                
            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: UpdateLocalLeverageQuantity -- " +   e.ToString());
            }
        }


        // ----------------------------------------------------------
        public int GetLocalLeverageQuantity(string trader, string instrument, string algoTrader)
        {
            int rtnVal = 0;

            try
            {
                /*
                var entities = new AlgoXRef4Entities();

                List<TraderXRef> traderData = (from t in entities.TraderXRefs
                                               where t.TraderID == trader
                                                     && t.Instrument == instrument
                                                        && t.AlgoTrader == algoTrader
                                               select t).ToList();
                if (traderData.Count() > 0)
                {
                    rtnVal = (int)traderData.First().LevQuantity;
                }
                */
            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: GetLocalLeverageQuantity -- " + e.ToString());
            }

            return rtnVal;
        }


        // ----------------------------------------------------------
        public int GetTotalLocalLeverageQuantity( string instrument, string algoTrader)
        {
            int rtnVal = 0;

            try
            {
                /*
                var entities = new AlgoXRef4Entities();

                List<TraderXRef> traderData = (from t in entities.TraderXRefs
                                               where t.Instrument == instrument
                                                        && t.AlgoTrader == algoTrader
                                               select t).ToList();
                if (traderData.Any())
                {
                    rtnVal = (int)traderData.Sum(x => x.LevQuantity);
                }

                */
            }

            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: GetTotalLocalLeverageQuantity -- " + e.ToString());
            }

            return rtnVal;
        }



        // ----------------------------------------------------------
        // Summary Functions
        // ----------------------------------------------------------
        private int GetServerActualsSummary(string algoTrader, string instrument)
        {
            int actualTotal = 0;

            try
            {

                /*
                var loh = new LiveOrderHelper();
                List<OrderData> openOrderList = loh.GetOrdersByTrader(algoTrader);

                IEnumerable<OrderData> specOrders = from spec in openOrderList
                                                    where spec.Instrument == instrument
                                                    select spec;


                foreach (OrderData ord in specOrders)
                {
                    if (ord.OrderAction < 0)
                    {
                        actualTotal = actualTotal + (ord.Quantity * -1);
                    }
                    else
                        actualTotal = actualTotal + (ord.Quantity);
                }
                */
            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: GetServerActualsSummary -- " + e.ToString());
            }

            return actualTotal;
        }

        // ----------------------------------------------------------
        private int GetLocalLeverageSummary(string algoTrader, string instrument)
        {
           
            int iXref = 0;


            try
            {
                /*
            var entities = new AlgoXRef4Entities();

            var xrefData = (from t in entities.TraderXRefs
                            where t.Instrument == instrument
                                    && t.AlgoTrader == algoTrader
                            group t by new { t.Instrument }
                                into xrefs
                                select new { xrefs.Key.Instrument, Qty = xrefs.Sum(y => y.LevQuantity) }).ToList();

                if (xrefData.Any())
                {
                    try
                    {
                        iXref = Convert.ToInt32(xrefData[0].Qty);
                    }
                    catch (Exception ie)
                    {
                        csw.LogRedType(10000, ie.ToString());
                    } 
                }
            */

            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: GetLocalLeverageSummary -- " + e.ToString());
            }

           

            return iXref;
        }

        // ----------------------------------------------------------
        public void ShowFullLeverageActualSummary(string algoTrader)
        {

            try
            {

                string x = "  ";    


                 // -------------------------
                // Check Actuals
                // -------------------------

                /*

                var loh = new LiveOrderHelper();
                List<OrderData> openOrderList = loh.GetOrdersByTrader(algoTrader);


                List<string> actInstruments = (from x in openOrderList
                                               select x.Instrument).Distinct().ToList();

                var entities = new AlgoXRef4Entities();
                List<string> xrefInstruments = (from t in entities.TraderXRefs
                                                select t.Instrument).ToList();

                List<string> comp = actInstruments.Concat(xrefInstruments).ToList();

                csw.Log(1,"  ");
                csw.LogGreenType(1, "----------------------------------------- ");
                csw.LogGreenType(1, "             Leverage Summary              ");
                csw.LogGreenType(1, "----------------------------------------- ");
                csw.LogGreenType(1, "Instrument      Leveraged       Actual ");
                csw.LogGreenType(1, "----------------------------------------- ");

                foreach (string inst in comp.Distinct())
                {
                    int lls = GetLocalLeverageSummary(algoTrader, inst);
                    int sas = GetServerActualsSummary(algoTrader, inst);

                    if (lls == sas)
                    {
                        csw.LogGreenType(1, inst + "          " + lls + "          " + sas);    
                    }
                    else
                    {
                        csw.LogRed(1, inst + "          " + lls + "          " + sas);
                    }

                }

                csw.LogGreen(1, "----------------------------------------- ");
                csw.Log(1, "  ");
            */
            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: ShowFullLeverageActualSummary -- "  + e.ToString());
            }
           
        }


        public string GetFullLeverageActualSummary(string algoTrader)
        {
            
            StringBuilder flas = new StringBuilder();

            // -------------------------
            // Check Actuals
            // -------------------------

            try
            {

                /*
                    var loh = new LiveOrderHelper();
                    List<OrderData> openOrderList = loh.GetOrdersByTrader(algoTrader);

                    List<string> actInstruments = (from x in openOrderList
                                                    select x.Instrument).Distinct().ToList();
                    var entities = new AlgoXRef4Entities();
                    List<string> xrefInstruments = (from t in entities.TraderXRefs
                                                    select t.Instrument).ToList();
                    List<string> comp = actInstruments.Concat(xrefInstruments).ToList();

                    foreach (string inst in comp.Distinct())
                    {
                        flas.Append(inst + "&" + GetLocalLeverageSummary(algoTrader, inst) + "&" +
                                    GetServerActualsSummary(algoTrader, inst));

                        flas.Append("|");
                    }

                 */   
            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: GetFullLeverageActualSummary -- "  + e.ToString());
            }



           

            return flas.ToString();

        }



        // ----------------------------------------------------------
        private void CheckLocalLeverageVersusActual(int OID, string trader, string instrument, string algoTrader,
                                                    string message)
        {

            try
            {
                int actualTotal = GetServerActualsSummary(algoTrader, instrument);
                int iXref = GetLocalLeverageSummary(algoTrader, instrument);

                csw.Log(1, " ");
                csw.Log(1, "Order: " + message);
                csw.Log(1, "Leverage Balance Summary for " + instrument + "     Order: " + OID);
                csw.Log(1, "Local Engine Balance: " + iXref + "   Server Order Balance: " + actualTotal);

                if (iXref != actualTotal)
                {
                    string msg = " WARNING: Quantity Imbalance ";
                    csw.LogRedType(1, msg);

                    //MailAdapter ma = new MailAdapter();
                    //ma.SendEmailMessage("contact@blackwavetechnologies.com", subj, msg);
                }

                csw.Log(1, " ");
            }
            catch (Exception e)
            {

                csw.LogRedType(10000, "Error: CheckLocalLeverageVersusActual -- "  + e.ToString());
            }
           
        }


        // ----------------------------------------------------------
        // Mirror Server XREF
        // ----------------------------------------------------------
        public void UpdateAllServerXRef(string algoTrader)
        {

            /*    
            var leh = new LeverageEngineHelper();
            leh.ClearAllServerXRefs(algoTrader);

            try
            {
                var entities = new AlgoXRef4Entities();

                List<TraderXRef> traderData = (from t in entities.TraderXRefs
                                               select t).ToList();

                if (traderData.Count() > 0)
                {
                    // row exists
                    foreach (TraderXRef traderXref in traderData)
                    {
                        try
                        {
                            leh.UpdateServerXRef(traderXref.TraderID, algoTrader, traderXref.Instrument,
                                                 (int)traderXref.ActualQuantity, (int)traderXref.LevQuantity);
                        }
                        catch (Exception eSave)
                        {
                            csw.LogRed(1, " --------------------------------------------------- ");
                            csw.LogRed(1, "Error Accessing Record " + eSave);
                            csw.LogRed(1, " --------------------------------------------------- ");
                        }
                    }
                }
            }
            catch (Exception eeee)
            {
                csw.LogRed(1, " --------------------------------------------------- ");
                csw.LogRed(1, "Error updating server XREF: " + eeee);
                csw.LogRed(1, " --------------------------------------------------- ");
            }
            */
        }


        // ----------------------------------------------------------
        public void UpdateServerXRef(string algoTrader, string trader, string instrument)
        {

            /*)
            var leh = new LeverageEngineHelper();

            try
            {
                var entities = new AlgoXRef4Entities();

                List<TraderXRef> traderData = (from t in entities.TraderXRefs
                                               where t.TraderID == trader
                                                     && t.Instrument == instrument
                                                        && t.AlgoTrader == algoTrader
                                               select t).ToList();

                if (traderData.Count() > 0)
                {
                    // row exists
                    foreach (TraderXRef traderXref in traderData)
                    {
                        try
                        {
                            leh.UpdateServerXRef(traderXref.TraderID, algoTrader, traderXref.Instrument,
                                                 (int)traderXref.ActualQuantity, (int)traderXref.LevQuantity);
                        }
                        catch (Exception eSave)
                        {
                            csw.LogRed(1," --------------------------------------------------- ");
                            csw.LogRed(1,"Error Accessing Record " + eSave);
                            csw.LogRed(1, " --------------------------------------------------- ");
                        }
                    }
                }
            }
            catch (Exception eeee)
            {
                csw.LogRed(1, " --------------------------------------------------- ");
                csw.LogRed(1,"Error updating server XREF: " + eeee);
                csw.LogRed(1, " --------------------------------------------------- ");
            }
            */

        }
    }
}