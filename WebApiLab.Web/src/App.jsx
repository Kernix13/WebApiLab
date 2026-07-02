import { useState, useEffect } from 'react'
import './App.css'

function App() {
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch('http://localhost:5195/api/People');

        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const result = await response.json();
        setData(result);
      } catch (error) {
        setError(error.message);
      } finally {
        setLoading(false);
      }

    };
    fetchData();

  }, []);

  if (loading) {
    return <p>Loading...</p>
  } else if (error) {
    return <p>Error: {error}</p>
  } else if (data) {
    return (
      <div>
        <h2>Fetched data:</h2>
        <pre>{JSON.stringify(data, null, 2)}</pre>
      </div>
    )
  }
}

export default App;
