SELECT 
    UP."DisplayName",
    SC."UserID",
    SC."GroupID",
    SC."Trades",
    SC."Winners",
    SC."Losers",
    SC."TotalNetProfit",
    SC."WinLossRatio",
    SC."PNL_Last3",
    SC."PNL_Last5",
    SC."PNL_Last8"
FROM 
    public."ScoreCard" AS SC
INNER JOIN 
    public."UserProfiles" AS UP ON SC."UserID" = UP."UserID"
ORDER BY 
    SC."TotalNetProfit" DESC;