// OrdersComponent.js

import React, { useState, useEffect } from 'react';

const LiveOrdersComponent = () => {
  const [orders, setOrders] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  var [date,setDate] = useState(new Date());

  useEffect(() => {
    const userInfo = {
      userId: 999999,
      password: "abc", // Note: Be cautious with sensitive information
      groupNum: 0
    };
  
    var timer = setInterval(()=>setDate(new Date()), 500 );

    

    const fetchOrders = () => {
      fetch('/api/data/get-live-orders', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(userInfo)
      })
      .then(response => {
        if (!response.ok) {
          throw new Error('Network response was not ok');
        }
        return response.json();
      })
      .then(data => setOrders(data))
      .catch(error => console.error('Error fetching live orders:', error));
    };
  
    // Initial fetch
    fetchOrders();
  
    // Set up polling
    const intervalId = setInterval(fetchOrders, 5000); // Adjust polling interval as needed (e.g., 5000 ms for 5 seconds)
  
    // Clean up
    return () => clearInterval(intervalId);
  }, []); 
  

  if (isLoading) return <div>Loading orders...</div>;
  if (error) return <div>Error fetching orders: {error}</div>;

  return (
    <div>
      <h2>Orders</h2>
      <p>Live orders are displayed here.</p>
      <div>
            <p> Time : {date.toLocaleTimeString()}</p>
            <p> Date : {date.toLocaleDateString()}</p>

        </div>
      <ul>
        {orders.map(order => (
          <div key={order.liveOrderID}>
            User: {order.userID}, Group {order.userGroup}, Order ID: {order.orderManagerID}, Price {order.orderPX}, Quantity: {order.quantity}, Action: {order.orderAction}
          </div>
        ))}
      </ul>
    </div>
  );
};

export default LiveOrdersComponent;
