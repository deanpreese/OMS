import React, { useEffect, useState } from 'react';
import * as signalR from "@microsoft/signalr";

function LiveOrdersComponent() {

    const [liveOrders, setLiveOrders] = useState([]);
    const [connectedOrders, setConnectLiveOrders] = useState([]);


    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:8786/hubs/signal-hub")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        connection.start()
            .then(() => {
                console.log("Connected to the order hub.");
                setConnectLiveOrders("Live orders connected");
            })
            .catch(err => console.error('Error connecting to the order hub: ', err));


        connection.on("ReceiveLiveOrders", (liveOrders) => {
            setLiveOrders(liveOrders);
            console.log(liveOrders);
        });

        return () => {
            connection.stop();
        }
    }, []);


    return (
        <div>
            <h2>Live Orders</h2>
            <h4>{connectedOrders}</h4>
            <ul>
                {liveOrders}
            </ul>
        </div>
    );
}

export default LiveOrdersComponent;
