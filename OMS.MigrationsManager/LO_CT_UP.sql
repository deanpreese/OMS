select "UserID", "GroupID", "PlatformOrderID", "RelatedOrderID", "OrderAction"
from public."LiveOrder" ;

select "UserID", "GroupID", "OpenPlatformOrderID", "OpenRelatedOrderID", 
"ClosePlatformOrderID", "CloseRelatedOrderID"
from public."ClosedTrades"