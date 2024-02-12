// TraderInfoListComponent.js

import React, { useState, useEffect } from 'react';

const TraderInfoListComponent = () => {
  const [traderInfos, setTraderInfos] = useState([]);

  useEffect(() => {
    fetch('/api/traderinfos') // Adjust the endpoint as needed.
      .then(response => response.json())
      .then(data => setTraderInfos(data))
      .catch(error => console.error('Error fetching trader infos:', error));
  }, []);

  return (
    <div>
      <h2>Trader Infos</h2>
      <ul>
        {traderInfos.map(info => (
          <li key={info.id}>
            Trader ID: {info.id}, Name: {info.name}, Balance: {info.balance}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default TraderInfoListComponent;
