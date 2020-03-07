import React, {useContext, useEffect, useState} from 'react';
import {Grid, Loader} from 'semantic-ui-react';
import ActivityList from './ActivityList';
import {observer} from 'mobx-react-lite';
// import LoadingComponent from '../../../app/layout/LoadingComponent';
import {RootStoreContext} from "../../../app/stores/rootStore";
import InfiniteScroll from 'react-infinite-scroller'
import ActivityFilter from "./ActivityFilter";
import ActivityListItemPlaceholder from "./ActivityItemListPlaceholder";

const ActivityDashboard: React.FC = () => {

    const rootStore = useContext(RootStoreContext); // fetching root store 
    const {loadActivities, loadingInitial, setPage, page, totalPages} = rootStore.activityStore; // de-structuring the props 
    const [loadingNext, setLoadingNext] = useState(false); // setting local state

    // this is something handling the next reloading piece of data 
    const handleGetNext = () => {
        setLoadingNext(true);
        setPage(page + 1);
        loadActivities().then(() => setLoadingNext(false))
    };

    // on component load - this hook comes alike
    useEffect(() => {
        loadActivities();
    }, [loadActivities]);

    // conditional rendering of Loading component
    // if (loadingInitial && page === 0) {
    //     return <LoadingComponent content='Loading activities'/>;
    // }

    return (
        <Grid>
            <Grid.Column width={10}>
                {loadingInitial && page === 0 ? <ActivityListItemPlaceholder/> : (
                    <InfiniteScroll pageStart={0} loadMore={handleGetNext}
                                    hasMore={!loadingNext && page + 1 < totalPages}
                                    initialLoad={false}>
                        <ActivityList/>
                    </InfiniteScroll>
                )}

                {/*<Button floated={'right'} content={'More...'} positive onClick={handleGetNext} loading={loadingNext}*/}
                {/*        disabled={totalPages === page + 1}/>*/}
            </Grid.Column>
            <Grid.Column width={6}>
                <ActivityFilter/>
            </Grid.Column>
            <Grid.Column width={10}>
                <Loader active={loadingNext}/>
            </Grid.Column>
        </Grid>
    );
};

export default observer(ActivityDashboard);
