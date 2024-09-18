import React, { useState } from 'react';
import './App.css';
import DataTable from './Components/DataTable';
import SearchAppBar from './Components/SearchAppBar';
import CircularProgress from '@mui/material/CircularProgress';

function App() {
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(false);

  const handleSearch = (data) => {
    setRows(data);
  };

  return (
    <div className="App">
      <SearchAppBar onSearch={handleSearch} setLoading={setLoading} />
      <DataTable rows={rows} />
      {loading && <CircularProgress size={120} style={{ marginLeft: 10 }} />}
    </div>
  );
}

export default App;