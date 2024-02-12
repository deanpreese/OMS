import React, { useEffect, useState } from 'react';
import * as signalR from "@microsoft/signalr";

function ClosedOrdersComponent() {
    
    const [closedOrders, setClosedOrders] = useState([]);
    const [connectedClosedOrders, setConnectClosedOrders] = useState([]);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:8786/hubs/signal-hub")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        connection.start()
            .then(() => {
                console.log("Connected to the order hub.");
                setConnectClosedOrders("Closed orders connected ");
            })
            .catch(err => console.error('Error connecting to the order hub: ', err));

        connection.on("ReceiveClosedTrades", (closedOrders) => {
            setClosedOrders(closedOrders);
            console.log(closedOrders);
        });
        
        return () => {
            connection.stop();
        }
    }, []);

    return (
        <div>
            <h2>Closed Orders </h2>
            <h4>{connectedClosedOrders}</h4>
            <ul>
                {closedOrders}
            </ul>
        </div>
    );
}

export default ClosedOrdersComponent;
