import React from "react";
import { createRoot } from "react-dom/client";
import "./style.css";
function App(){return <main><header><b>FreshCart</b><nav>Shop　 Categories　 Sign in</nav></header><section className="hero"><p>FRESH FROM LOCAL SHOPS</p><h1>Good food, picked fresh.</h1><p>Discover seasonal fruits and vegetables from shops near you.</p><button>Explore produce</button></section><section><h2>Browse categories</h2><div className="grid">{["Fresh Fruits","Vegetables","Leafy Greens"].map(x=><article key={x}>{x}</article>)}</div></section><footer>FreshCart · Local freshness, delivered</footer></main>}
createRoot(document.getElementById("root")).render(<App/>);
