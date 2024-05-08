SELECT 
	SC."GroupID",
    Sum(SC."TotalNetProfit") as grp_pft,
	SC."WinLossRatio"
FROM 
    public."ScoreCards" AS SC
	
INNER JOIN 
    public."UserProfiles" AS UP ON SC."UserID" = UP."UserID"

where SC."GroupID" > 100	
group by 
	SC."GroupID", SC."WinLossRatio"

order by "WinLossRatio" desc