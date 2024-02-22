using Algo.Algorithms.Abstractions;
using Algo.Algorithms.Models;
using OMS.Core.Models;


namespace Algo.Algorithms.Filters;

public class NewAlgoFilter : AbstractAlgoFilter, IAlgoFilter
{

    public NewAlgoFilter(UserProfile userProfile, ScoreCard scoreCard) : base(userProfile, scoreCard) 
    {
    }

    public int IsInAlgoFilter()
    {
        int includeExclude = 0;
               
        if (_scoreCard.PNL_Last3  > 0)  { includeExclude = 1; }  
        if (_scoreCard.PNL_Last5  > 0)  { includeExclude = 1; }
        if (_scoreCard.PNL_Last8  > 0 ) { includeExclude =  0; }

        return includeExclude;
    }

  public int IsInAlgoFilter99(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        //Winning Trader overall
        if(_scoreCard.GrossProfit > Math.Abs((double)_scoreCard.GrossLoss))
        {
            includeExclude = 1;
        }else
        {
            double Set3 =  _scoreCard.PNL_Last3;
            double Set5 =  _scoreCard.PNL_Last5;
            double Set13 = _scoreCard.PNL_Last13;

            if( Set5 < Set13 && Set3 < Set5)
            {
                if(Set5 < 0)        
                {
                   includeExclude = -1; 
                }
            }
                
            if( Set5 > Set13 && Set3 > Set5) 
            {
                if(Set5 > 0)        
                {
                   includeExclude = 1; 
                }
            }
            
        }
        
        return includeExclude;
    }
 

    public int IsInAlgoFilter98(int traderId, int groupNumber)
    {
        int includeExclude = 0;
        
        if ( _scoreCard.Winners > _scoreCard.Losers)
        {
            if(_scoreCard.GrossProfit > Math.Abs((double)_scoreCard.GrossLoss))
            {
                includeExclude = 1;
            }

        }else
        {
            double Set3 =  _scoreCard.PNL_Last3;
            double Set5 =  _scoreCard.PNL_Last5;
            double Set13 = _scoreCard.PNL_Last13;

            if(_scoreCard.WinLossRatio < .45)
            {
                // 5 SMA < 13 SMA -->>  Losing
                if( Set5 < Set13 && Set3 < Set5)
                {
                    if(Set5 < 0)        
                    {
                    includeExclude = -1; 
                    }
                }
                    
                // 5 SMA > 13 SMA -->>  Winning
                if( Set5 > Set13 && Set3 > Set5) 
                {
                    if(Set5 > 0)        
                    {
                    includeExclude = 1; 
                    }
                }
            }
            
        }

        
        return includeExclude;
    }


    public int IsInAlgoFilter97(int traderId, int groupNumber)
    {
        int includeExclude = 0;
  

        if ( _scoreCard.Winners > _scoreCard.Losers)
        {
            if(_scoreCard.GrossProfit > Math.Abs((double)_scoreCard.GrossLoss))
            {
                includeExclude = 1;
            }

        }else
        {
            double Set3 =  _scoreCard.PNL_Last3;
            double Set5 =  _scoreCard.PNL_Last5;
            double Set13 = _scoreCard.PNL_Last13;

            if(_scoreCard.WinLossRatio < .45)
            {
                // 5 SMA < 13 SMA -->>  Losing
                if( Set5 < Set13 && Set3 < Set5)
                {
                    if(Set5 < 0)        
                    {
                    includeExclude = -1; 
                    }
                }

            }

            if(_scoreCard.WinLossRatio > .5 )                        
            {
                // 5 SMA > 13 SMA -->>  Winning
                if( Set5 > Set13 && Set3 > Set5) 
                {
                    if(Set5 > 0)        
                    {
                    includeExclude = 1; 
                    }
                }
            }
        }
        
        return includeExclude;
    }

              

}
