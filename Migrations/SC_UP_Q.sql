SELECT 
    UP."DisplayName",
    SC."ScoreCardID",
    SC."GroupID",
    SC."Trades",
    SC."Winners",
    SC."Losers",
    SC."SharpRatio",
    SC."SharpRatio",
    SC."TotalNetProfit",
    SC."WinLossRatio"
FROM 
    public."ScoreCard" AS SC
INNER JOIN 
    public."UserProfiles" AS UP ON SC."UserID" = UP."UserID"
ORDER BY 
    SC."WinLossRatio" DESC;