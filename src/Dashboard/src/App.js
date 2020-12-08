import React from 'react';
import { HubConnectionBuilder } from '@aspnet/signalr';
import {createGlobalStyle} from 'styled-components'
import {Row, Col, Container} from 'react-bootstrap'
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const GlobalStyle = createGlobalStyle`
  html, body {
    background-color: #ffffff;
  }
`;

class App extends React.Component
{
  constructor(props) 
  {
      super();

      this.state = {
        websocket: null,
        alert: {
          speedLimit: 30,
          currentSpeed: 0
        }
      }
  }

  componentDidMount() {
    var signalrURL = process.env.REACT_APP_HUB_URL
    if (signalrURL && signalrURL !== "")
    {
      var hubConnectionRef = new HubConnectionBuilder()
        .withUrl(signalrURL)
        .build();

      hubConnectionRef.start().then(
        this.setState({
          websocket: hubConnectionRef
        })
      ).then(
        hubConnectionRef.on('alerts', (alert) => {
          var alertJson = JSON.parse(alert)
          this.setState({
              alert: alertJson
          })
        })
      )
    }
  }

  render() {
    if (this.state.websocket !== null) {
      return (
          <React.Fragment>
            <ToastContainer />
            <GlobalStyle />
            <Container style={{padding: '0px'}} fluid>
              <div style={{textAlign: 'center'}}>
                <div style={{float: 'center'}}>Current Speed</div>
                <div style={{fontSize: '40pt'}}>{this.state.alert.currentSpeed}</div>
                <div>Speed Limit</div>
                <div style={{fontSize: '40pt'}}>{this.state.alert.speedLimit}</div>
              </div>
            </Container>
          </React.Fragment>
        )
      }
      return (
        <div>Waiting for websocket to connect...</div>
      )
  }
}

export default App;
