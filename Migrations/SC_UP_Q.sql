SELECT 
    UP."DisplayName",
    SC.*
FROM 
    public."ScoreCard" AS SC
INNER JOIN 
    public."UserProfiles" AS UP ON SC."UserID" = UP."UserID"
ORDER BY 
    SC."TotalNetProfit" DESC;