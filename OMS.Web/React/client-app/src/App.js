
import React from 'react';
import LiveOrdersComponent from './LiveOrdersComponent'; // Adjust the import path based on your file structure
//import ClosedTradesComponent from './ClosedTradesComponent'; // Adjust the import path based on your file structure
//import TraderInfoListComponent from './TraderInfoListComponent'; // Adjust the import path based on your file structure

import logo from './logo.svg';
import './App.css';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1>Trading Dashboard</h1>
      </header>
      <div>
        <LiveOrdersComponent />
    </div>
    </div>
  );
}

export default App;
