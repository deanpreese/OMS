SELECT 
    UP."DisplayName",
    UP."UserID",
	SC."GroupID",
    SC."Trades",
    SC."Winners",
    SC."Losers",
    SC."TotalNetProfit",
    SC."WinLossRatio"
FROM 
    public."ScoreCards" AS SC
	
INNER JOIN 
    public."UserProfiles" AS UP ON SC."UserID" = UP."UserID"



ORDER BY 
    SC."WinLossRatio" DESC;
