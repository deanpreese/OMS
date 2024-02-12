
import './App.css';
import ClosedOrdersComponent from './ClosedOrdersComponent';
import LiveOrdersComponent from './LiveOrdersComponent';


function App() {

  return (
    <div>
      <LiveOrdersComponent />
      <ClosedOrdersComponent />
    </div>
    );
}
export default App;
