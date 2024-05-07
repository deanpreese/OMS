SELECT 
	SC."GroupID",
    Sum(SC."TotalNetProfit") as grp_pft
FROM 
    public."ScoreCards" AS SC
	
INNER JOIN 
    public."UserProfiles" AS UP ON SC."UserID" = UP."UserID"

group by 
	SC."GroupID"

order by "GroupID" 