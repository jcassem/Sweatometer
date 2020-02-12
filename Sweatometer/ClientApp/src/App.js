import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FindApp } from './components/FindApp';
import { MergeApp } from './components/MergeApp';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
            <Route exact path='/' component={Home} />
            <Route path='/find' component={FindApp} />
            <Route path='/merge' component={MergeApp} />
      </Layout>
    );
  }
}
