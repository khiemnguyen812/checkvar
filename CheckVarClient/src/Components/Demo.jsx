import React, { useState, useEffect } from 'react';

function ClickCounter() {
  const [clicks, setClicks] = useState(0);

  useEffect(() => {
    console.log(`The number of clicks is now: ${clicks}`);

    // Optionally, you can return a cleanup function here
    return () => {
      console.log('Cleaning up...');
    };
  }, [clicks]); // Only re-run the effect if clicks changes

  return (
    <div>
      <p>You clicked {clicks} times</p>
      <button onClick={() => setClicks(clicks + 1)}>Click me!</button>
    </div>
  );
}

export default ClickCounter;
