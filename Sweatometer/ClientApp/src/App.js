import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FindApp } from './components/find/FindApp';
import { MergeApp } from './components/merge/MergeApp';
import { SweatTestApp } from './components/sweat/SweatTestApp';
import { EmojiResultTable } from './components/emoji/EmojiResultTable';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
            <Route exact path='/' component={Home} />
            <Route path='/find' component={FindApp} />
            <Route path='/merge' component={MergeApp} />
            <Route path='/sweat' component={SweatTestApp} />
            <Route path='/emoji' component={EmojiResultTable} />
      </Layout>
    );
  }
}
