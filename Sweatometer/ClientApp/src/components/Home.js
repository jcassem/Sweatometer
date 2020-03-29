import React, { Component } from 'react';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (
            <div>
                <main role="main" class="inner cover">
                    <h1 class="cover-heading"><i>Sweat</i>: how hard is your pun working?</h1>
                    <p class="lead">Sweat describes the level of effort a forced combination
                            of words (like puns) has been worked itself into existance.</p>
                    <p class="lead">E.g. <i>Wookiee Moster</i> (Cookie Monster x Wookiee) is GOLD and would
                            therefore have a low sweat score.</p>
                    <p class="lead">Whereas <i>Buddha Fett</i> (Boba Fett x Buddha) has a much higher
                            sweat-level.</p>
                </main>
                <div>
                    <h4>Try out one of our services</h4>
                    <a type="button" class="btn btn-lg btn-block btn-outline-primary" href="/find">Find similar words</a>
                    <a type="button" class="btn btn-lg btn-block btn-outline-primary" href="/merge">Create a pun (word merge)</a>
                    <a type="button" class="btn btn-lg btn-block btn-outline-primary" href="/sweat">Sweat Test your pun</a>
                </div>
            </div>
        );
  }
}
