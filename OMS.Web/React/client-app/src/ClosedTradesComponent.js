// ClosedTradesComponent.js

import React, { useState, useEffect } from 'react';

const ClosedTradesComponent = () => {
  const [closedTrades, setClosedTrades] = useState([]);

  useEffect(() => {
    fetch('/api/get-closed-trades') // Adjust the endpoint as needed.
      .then(response => response.json())
      .then(data => setClosedTrades(data))
      .catch(error => console.error('Error fetching closed trades:', error));
  }, []);

  return (
    <div>
      <h2>Closed Trades</h2>
      <ul>
        {closedTrades.map(trade => (
          <li key={trade.id}>
            Trade ID: {trade.id}, Profit: {trade.profit}, Date Closed: {trade.dateClosed}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ClosedTradesComponent;
